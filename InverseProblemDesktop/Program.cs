using Application.DirectProblem._2D.Cylindrical.Harmonic;
using Application.EquationSystems;
using Application.EquationSystems.MatrixDecompositions.LU;
using Application.EquationSystems.Preconditions.Separate;
using Application.EquationSystems.Solvers;
using Application.EquationSystems.Solvers.Sparse;
using Application.FEM._2D.Assembling.Boundaries.First;
using Application.InverseProblem;
using Application.InverseProblem._2D.Harmonic;
using Application.InverseProblem.Assembling.Concurrent;
using Application.InverseProblem.Assembling.Derivatives._2D.Harmonic;
using Application.InverseProblem.Assembling.Harmonic;
using Application.InverseProblem.Assembling.Regularization.Alpha._2D;
using Application.InverseProblem.Parameters;
using Application.IO.Grid;
using Application.IO.Measurements;
using Domain;
using System.Diagnostics;
using System.Globalization;
using System.Numerics;
using Tests;

Thread.CurrentThread.CurrentCulture = CultureInfo.InvariantCulture;

var trueGridParameters = TestGridParameters.GetGridWith0Dot003125StepWithElementCloseToWellWith8Materials();
var trueMaterials = TestMaterials.GetMaterialsForGridWith0Dot003125StepWith8Materials();
var frequencies = TestFrequencies.GetOneFrequency().Select(f => f * 2 * Math.PI).ToArray();

const int sourcePower = 1;

var (sources, receiverLines) = TestReceiverAndSourceConstructions.GetAllConstructions(sourcePower);

var trueMeasurements = new double[frequencies.Length, sources.Length];

var targetParameters = new Parameter[]
{
    //new() { Type = ParameterType.Sigma, Index = 0, InitialValue = 0.1 },

    //new() { Type = ParameterType.Sigma, Index = 1, InitialValue = 0.1 },
    //new() { Type = ParameterType.Sigma, Index = 2, InitialValue = 0.1 },
    //new() { Type = ParameterType.Sigma, Index = 3, InitialValue = 0.1 },
    //new() { Type = ParameterType.Sigma, Index = 4, InitialValue = 0.1 },
    //new() { Type = ParameterType.Sigma, Index = 5, InitialValue = 0.1 },
    //new() { Type = ParameterType.Sigma, Index = 6, InitialValue = 0.1 },
    //new() { Type = ParameterType.Sigma, Index = 7, InitialValue = 0.1 },

    //new() { Type = ParameterType.VerticalBound, Index = 2, InitialValue = 1.5 },
    //new() { Type = ParameterType.VerticalBound, Index = 3, InitialValue = 2.1 },

    new() { Type = ParameterType.HorizontalBound, Index = 1, InitialValue = -4.5 },
    new() { Type = ParameterType.HorizontalBound, Index = 2, InitialValue = -3.75 },
    new() { Type = ParameterType.HorizontalBound, Index = 3, InitialValue = -3.5 },
    new() { Type = ParameterType.HorizontalBound, Index = 4, InitialValue = -3.25 },
    new() { Type = ParameterType.HorizontalBound, Index = 5, InitialValue = -2.5 },

    //new() { Type = ParameterType.HorizontalBound, Index = 1, InitialValue = -4.5 },
    //new() { Type = ParameterType.HorizontalBound, Index = 2, InitialValue = -4.25 },
    //new() { Type = ParameterType.HorizontalBound, Index = 3, InitialValue = -3.75 },
    //new() { Type = ParameterType.HorizontalBound, Index = 4, InitialValue = -3.5 },
    //new() { Type = ParameterType.HorizontalBound, Index = 5, InitialValue = -3.25 },
    //new() { Type = ParameterType.HorizontalBound, Index = 6, InitialValue = -2.75 },
    //new() { Type = ParameterType.HorizontalBound, Index = 7, InitialValue = -2.5 },

    //new() { Type = ParameterType.HorizontalBound, Index = 1, InitialValue = -3.5 },
    //new() { Type = ParameterType.HorizontalBound, Index = 3, InitialValue = -2.5 },
};

const int maxPossibleThreads = 10;
var maxThreads = targetParameters.Length > maxPossibleThreads ? targetParameters.Length : maxPossibleThreads;
var frequenciesParallelOptions = new ParallelOptions { MaxDegreeOfParallelism = frequencies.Length };
var parametersParallelOptions = new ParallelOptions { MaxDegreeOfParallelism = maxThreads };

var luIncompleteDecomposition = new LUIncompleteDecomposition();
var iterativeMethodConfig = new IterativeMethodConfig();

var directProblems = new HarmonicRotorDirectProblem2DFactory().Create
(
    () => new LocalOptimalScheme(new LUPrecondition(luIncompleteDecomposition), iterativeMethodConfig),
    //() => new LU(new LUDecomposition()),
    maxThreads > frequencies.Length ? maxThreads : frequencies.Length
);

var measurementCalculatorManager = new HarmonicMeasurementCalculatorManager(directProblems);

var firstBoundaries =
    new ComplexDefinedValueFirstBoundaryProvider2D().GetOnAllBounds(trueGridParameters, new Complex(0, 0));

var trueGrid = measurementCalculatorManager.GetAllFreeCalculators().First().GenerateGrid(trueGridParameters);

var sigmaCount = targetParameters.Count(p => p.Type == ParameterType.Sigma);
var verticalBoundCount = targetParameters.Count(p => p.Type == ParameterType.VerticalBound);
var horizontalBoundCount = targetParameters.Count(p => p.Type == ParameterType.HorizontalBound);

var writeBasePath =
    $"../../../Results/{sigmaCount} Sigmas {verticalBoundCount} VerticalBounds {horizontalBoundCount} HorizontalBounds {frequencies.Length} Frequencies {receiverLines.Length} Receivers/";

var gridWriter = new GridWriter2D(writeBasePath);
var measurementsWriter = new HarmonicMeasurementsWriter2D(writeBasePath);

Console.WriteLine("True measurements begin calculating");

//gridWriter.WriteMaterials(trueGrid, "nvkat2d.dat");
//gridWriter.WriteElements(trueGrid, "nvtr.dat");
//gridWriter.WriteNodes(trueGrid, "rz.dat");

var stopwatch = new Stopwatch();
stopwatch.Start();

Parallel.For(0, frequencies.Length, frequenciesParallelOptions, frequencyIndex =>
{
    var measurementCalculator = measurementCalculatorManager.WaitFreeCalculator();

    measurementCalculator.SetGrid(trueGrid);
    measurementCalculator.SetMaterials(trueMaterials);
    measurementCalculator.SetDefinedValueBoundaryCondition(firstBoundaries);
    measurementCalculator.SetFrequency(frequencies[frequencyIndex]);

    for (var j = 0; j < receiverLines.Length; j++)
    {
        measurementCalculator.SetSources([sources[j]]);

        var directProblemSolution = measurementCalculator.Solve();

        //measurementsWriter.WriteSinuses(directProblemSolution, trueGrid, "v2s.dat");
        //measurementsWriter.WriteCosinuses(directProblemSolution, trueGrid, "v2c.dat");

        var potentialM = directProblemSolution.Get(receiverLines[j].ReceiverM);
        var potentialN = directProblemSolution.Get(receiverLines[j].ReceiverN);

        trueMeasurements[frequencyIndex, j] = (potentialM.Phase - potentialN.Phase) * 180d / Math.PI;

        Console.Write($"Frequency {frequencyIndex} source {j}                                   \r");
    }

    measurementCalculatorManager.ReleaseCalculator(measurementCalculator);
});

stopwatch.Stop();

var time = (double)stopwatch.ElapsedMilliseconds / 1000;

gridWriter.WriteAreas(trueGridParameters, trueMaterials, "true areas.txt");
measurementsWriter.WriteMeasurements(receiverLines, trueMeasurements, frequencies, 0,"true measurements.txt");

Console.WriteLine();
Console.WriteLine("True measurements calculated");
Console.WriteLine($"Elapsed time {time}");

//foreach (var phaseDifference in trueMeasurements)
//{
//    Console.WriteLine(phaseDifference);
//}

var gridParameters = TestGridParameters.GetGridWith0Dot003125StepWithElementCloseToWellWith8Materials();
var materials = TestMaterials.GetMaterialsForGridWith0Dot003125StepWith8Materials();

var inverseProblemContextProvider = new HarmonicInverseProblem2DContextProvider();

var measurementDerivativesCalculators = directProblems
    .Select(p =>
        new HarmonicMeasurementDerivativesCalculator2D(inverseProblemContextProvider, p, boundParameterDelta: 0.0125))
    .ToArray<IHarmonicMeasurementDerivativesCalculator>();

var measurementDerivativesCalculatorManager = new HarmonicMeasurementDerivativeCalculatorManager(measurementDerivativesCalculators);
var equationAssembler = new ConcurrentHarmonicEquationAssembler2D(measurementDerivativesCalculatorManager, parametersParallelOptions);
var alphaRegularization = new AlphaRegularization2D
(
    inverseProblemContextProvider,
    new GaussElimination(),
    targetParameters,
    new Interval { Begin = 1e-3, End = 5 },
    2,
    0.1,
    0.125
);

var inverseProblem = new ConcurrentHarmonicInverseProblem2D
(
    inverseProblemContextProvider,
    measurementCalculatorManager,
    equationAssembler,
    alphaRegularization,
    new MinimizationMethodConfig(),
    gridWriter,
    measurementsWriter,
    frequenciesParallelOptions,
    0.0125
);

inverseProblem.SetGridParameters(gridParameters);
inverseProblem.SetMaterials(materials);
inverseProblem.SetDefinedValueBoundaryCondition(firstBoundaries);
inverseProblem.SetSourcesAndReceivers(sources, receiverLines);
inverseProblem.SetFrequencies(frequencies);
inverseProblem.SetTargetParametersAndMeasurements(targetParameters, trueMeasurements);

Console.WriteLine("Inverse problem begin solving");
stopwatch.Restart();

var inverseProblemSolution = inverseProblem.Solve();

stopwatch.Stop();

time = (double)stopwatch.ElapsedMilliseconds / 1000;

Console.WriteLine();
Console.WriteLine("Inverse problem solved");

for (var i = 0; i < inverseProblemSolution.Count; i++)
{
    Console.WriteLine($"Parameter {i} value {inverseProblemSolution[i]:F6}");
}

Console.WriteLine($"Elapsed time {time}");
