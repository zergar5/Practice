using DirectProblem;
using DirectProblem.Core.GridComponents;
using DirectProblem.GridGenerator;
using DirectProblem.GridGenerator.Intervals.Splitting;
using DirectProblem.TwoDimensional;
using DirectProblem.TwoDimensional.Assembling.Local;
using InverseProblem;
using InverseProblem.Assembling;
using InverseProblem.Parameters;
using InverseProblem.SLAE;
using System.Globalization;
using Vector = DirectProblem.Core.Base.Vector;

Thread.CurrentThread.CurrentCulture = CultureInfo.InvariantCulture;

var trueGrid = Grids.GetSmallGridWith0Dot003125StepWithElementCloseToWell();

var gridBuilder2D = new GridBuilder2D();

var current = 1d;
var mu = 4 * Math.PI * 10e-7;

var trueMaterials = new Material[]
{
    new(mu, 0.5),
    new(mu, 0.1),
    new(mu, 0.05),
    new(mu, 0.2),
    new(mu, 1d / 3d),
    new(mu, 0d),
    new(mu, 1d)
};

var frequencies = new[] { 4e4, 2e5, 1e6, 2e6 };

var sources = new Source[5];
var receiverLines = new ReceiverLine[sources.Length];
var truePhaseDifferences = new double[frequencies.Length, receiverLines.Length];

for (var i = 0; i < sources.Length; i++)
{
    sources[i] = new Source(new Node2D(0.05, -2.85 - 0.05 * i), current);
    receiverLines[i] = new ReceiverLine(
        new Node2D(sources[i].Point.R, sources[i].Point.Z - 0.05),
        new Node2D(sources[i].Point.R, sources[i].Point.Z - 0.1)
    );
}

var directProblemSolver = new DirectProblemSolver(trueGrid, trueMaterials);

var localBasisFunctionsProvider = new LocalBasisFunctionsProvider(trueGrid);

directProblemSolver.SetGrid(trueGrid).SetMaterials(trueMaterials);

for (var i = 0; i < frequencies.Length; i++)
{
    for (var j = 0; j < receiverLines.Length; j++)
    {
        var solution = directProblemSolver
            .SetFrequency(frequencies[i])
            .SetSource(sources[j])
            .AssembleSLAE()
            .Solve();

        var femSolution = new FEMSolution(trueGrid, solution, localBasisFunctionsProvider);

        var fieldM = femSolution.Calculate(receiverLines[j].PointM);
        var fieldN = femSolution.Calculate(receiverLines[j].PointN);

        truePhaseDifferences[i, j] = (fieldM.Phase - fieldN.Phase) * 180d / Math.PI;

        Console.Write($"frequency {i} source {j}                                      \r");
    }
}

Console.WriteLine();
Console.WriteLine("TrueDirectProblem calculated");

var gridParameters = new GridParameters
(
    [1e-4, 0.1, 1d, 3d],
    [-6d, -4d, -3d, -2d, 0d],
    [
        new UniformSplitter(16),
        new StepProportionalSplitter(0.003125, 1.1),
        new StepProportionalSplitter(0.1, 1.1)
    ],
    [
        new StepProportionalSplitter(0.003125, 1 / 1.1),
        new StepUniformSplitter(0.003125),
        new StepUniformSplitter(0.003125),
        new StepProportionalSplitter(0.003125, 1.1)
    ],
    [
        //скважина
        new(0, new Node2D(1e-4, -6d), new Node2D(0.1, 0d)),
        //первый слой
        new(2, new Node2D(0.1, -2d), new Node2D(10d, 0d)),
        //второй слой
        new(1, new Node2D(0.1, -2.75), new Node2D(1, -2d)),
        new(1, new Node2D(1, -3d), new Node2D(3d, -2d)),
        //искомый элемент
        new(4, new Node2D(0.1, -3.25), new Node2D(1, -2.75)),
        //третий слой
        new(3, new Node2D(0.1, -4d), new Node2D(1, -3.25)),
        new(3, new Node2D(1, -4d), new Node2D(3d, -3d)),
        //четвертый слой
        new(2, new Node2D(0.1, -6d), new Node2D(3d, -4d))
    ]
);

var materials = new Material[]
{
    new(mu, 0.49),
    new(mu, 0.19),
    new(mu, 0.04),
    new(mu, 0.09),
    new(mu, 1d / 3d - 0.01),
    new(mu, 0d),
    new(mu, 1d)
};

var targetParameters = new Parameter[]
{
    new (ParameterType.Sigma, 0, 0),
    new (ParameterType.Sigma, 1, 0),
    new (ParameterType.Sigma, 2, 0),
    new (ParameterType.Sigma, 3, 0),
    new (ParameterType.Sigma, 4, 0)
};

var trueValues = new Vector([0.5, 0.2]);
var initialValues = new Vector([0.49, 0.19, 0.04, 0.09, 1d / 3d - 0.01]);

var parametersCollection =
    new ParametersCollection(materials, gridParameters.RControlPoints, gridParameters.ZControlPoints);

var slaeAssembler = new SLAEAssembler(gridBuilder2D, directProblemSolver, localBasisFunctionsProvider,
    parametersCollection, gridParameters, sources, receiverLines, frequencies, targetParameters, initialValues,
    truePhaseDifferences);

var gaussElimination = new GaussElimination();

var regularizer = new Regularizer(gaussElimination, targetParameters);

var inverseProblemSolver = new InverseProblemSolver(gridBuilder2D, directProblemSolver, slaeAssembler, regularizer,
    gaussElimination, localBasisFunctionsProvider, parametersCollection, gridParameters, sources, receiverLines,
    frequencies, targetParameters, trueValues, truePhaseDifferences);

var parametersValues = inverseProblemSolver.Solve();

foreach (var parameterValue in parametersValues)
{
    Console.WriteLine(parameterValue);
}