using DirectProblem;
using DirectProblem.Core.GridComponents;
using DirectProblem.IO;
using DirectProblem.TwoDimensional;
using DirectProblem.TwoDimensional.Assembling.Local;
using InverseProblem;
using InverseProblem.Assembling;
using InverseProblem.Parameters;
using InverseProblem.SLAE;
using System.Diagnostics;
using System.Globalization;
using Vector = DirectProblem.Core.Base.Vector;

Thread.CurrentThread.CurrentCulture = CultureInfo.InvariantCulture;

var trueGrid = Grids.GetGridWith0Dot003125StepWithElementsCloseAndNearToWellAnd8Sigmas();

const double current = 1d;
const double mu = 4 * Math.PI * 1e-7;

var trueMaterials = new Material[]
{
    new(mu, 0.5),
    new(mu, 0.05),
    new(mu, 1d/30),
    new(mu, 0.01),
    new(mu, 1d / 3d),
    new(mu, 0.2),
    new(mu, 0.1),
    new(mu, 0.25)
};

var frequencies = new[] { 4e4, 2e5, 1e6, 2e6 };

var sources = new Source[10];
var receiverLines = new ReceiverLine[sources.Length];
var truePhaseDifferences = new double[frequencies.Length, receiverLines.Length];

for (var i = 0; i < sources.Length; i++)
{
    sources[i] = new Source(new Node2D(0.05, -2.5 - 0.1 * i), current);
    receiverLines[i] = new ReceiverLine(
        new Node2D(sources[i].Point.R, sources[i].Point.Z - 0.05),
        new Node2D(sources[i].Point.R, sources[i].Point.Z - 0.1)
    );
}

var maxThreads = 4;

var targetParameters = new Parameter[]
{
    new (ParameterType.Sigma, 0, 0),
    new (ParameterType.Sigma, 1, 0),
    new (ParameterType.Sigma, 2, 0),
    new (ParameterType.Sigma, 3, 0),
    new (ParameterType.Sigma, 4, 0),
    new (ParameterType.Sigma, 5, 0),
    new (ParameterType.Sigma, 6, 0),
    new (ParameterType.Sigma, 7, 0)
};

var trueValues = new Vector([0.5, 0.05, 1d / 30, 0.01, 1d / 3d, 0.2, 0.1, 0.25]);

DirectProblemSolver[] directProblemSolvers;
LocalBasisFunctionsProvider[] localBasisFunctionsProviders;

if (targetParameters.Length < maxThreads)
{
    directProblemSolvers = new DirectProblemSolver[targetParameters.Length];
    localBasisFunctionsProviders = new LocalBasisFunctionsProvider[targetParameters.Length];
}
else
{
    directProblemSolvers = new DirectProblemSolver[maxThreads];
    localBasisFunctionsProviders = new LocalBasisFunctionsProvider[maxThreads];
}

for (var i = 0; i < directProblemSolvers.Length; i++)
{
    directProblemSolvers[i] = new DirectProblemSolver(trueGrid, trueMaterials)
        .SetGrid(trueGrid).SetMaterials(trueMaterials);
    localBasisFunctionsProviders[i] = new LocalBasisFunctionsProvider(trueGrid);
}

var stopwatch = new Stopwatch();
stopwatch.Start();

var resultO = new ResultIO("../InverseProblem/Results/8OtherSigmasCloseAndNearToWell/");
var gridO = new GridIO("../InverseProblem/Results/8OtherSigmasCloseAndNearToWell/");

for (var i = 0; i < frequencies.Length; i++)
{
    for (var j = 0; j < receiverLines.Length; j++)
    {
        var solution = directProblemSolvers[0]
            .SetFrequency(frequencies[i])
            .SetSource(sources[j])
            .AssembleSLAE()
            .Solve();

        var femSolution = new FEMSolution(trueGrid, solution, localBasisFunctionsProviders[0]);

        var fieldM = femSolution.Calculate(receiverLines[j].PointM);
        var fieldN = femSolution.Calculate(receiverLines[j].PointN);

        truePhaseDifferences[i, j] = (fieldM.Phase - fieldN.Phase) * 180d / Math.PI;

        Console.Write($"frequency {i} source {j}                                      \r");
    }
}

resultO.WriteInverseProblemIteration(receiverLines, truePhaseDifferences, frequencies, "true phase differences.txt");
gridO.WriteAreas(trueGrid, trueValues, "true areas.txt");

Console.WriteLine();
Console.WriteLine("TrueDirectProblem calculated");
stopwatch.Stop();

var time = (double)stopwatch.ElapsedMilliseconds / 1000;

Console.WriteLine();
Console.WriteLine($"Elapsed time {time}");

var parametersCollections = new ParametersCollection[directProblemSolvers.Length];

for (var i = 0; i < parametersCollections.Length; i++)
{
    var materials = new Material[]
    {
        new(mu, 0.1),
        new(mu, 0.1),
        new(mu, 0.1),
        new(mu, 0.1),
        new(mu, 0.1),
        new(mu, 0.1),
        new(mu, 0.1),
        new(mu, 0.1)
    };

    parametersCollections[i] = new ParametersCollection(materials, [1e-4, 0.1, 1d, 3d], [-6d, -4d, -3d, -2d, 0d]);
}

var initialValues = new Vector([0.1, 0.1, 0.1, 0.1, 0.1, 0.1, 0.1, 0.1]);

var slaeAssembler = new SLAEAssembler(directProblemSolvers, localBasisFunctionsProviders,
    parametersCollections, sources, receiverLines, frequencies, targetParameters, initialValues,
    truePhaseDifferences);

var gaussElimination = new GaussElimination();

var regularizer = new Regularizer(gaussElimination, targetParameters);

var inverseProblemSolver = new InverseProblemSolver(directProblemSolvers, slaeAssembler, regularizer,
    gaussElimination, localBasisFunctionsProviders, trueGrid, parametersCollections, sources, receiverLines,
    frequencies, targetParameters, truePhaseDifferences, initialValues);

stopwatch.Restart();

inverseProblemSolver.Solve();

stopwatch.Stop();

time = (double)stopwatch.ElapsedMilliseconds / 1000;

Console.WriteLine();
Console.WriteLine($"Elapsed time {time}");