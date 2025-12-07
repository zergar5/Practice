using Application.DirectProblem;
using Application.DirectProblem._2D.Cylindrical.Harmonic;
using Application.EquationSystems;
using Application.EquationSystems.MatrixDecompositions.LU;
using Application.EquationSystems.Preconditions.Separate;
using Application.EquationSystems.Solvers;
using Application.EquationSystems.Solvers.Sparse;
using Application.FEM._1D.Assembling.Local;
using Application.FEM._2D;
using Application.FEM._2D.Assembling.Boundaries;
using Application.FEM._2D.Assembling.Boundaries.First;
using Application.FEM._2D.Assembling.Global;
using Application.FEM._2D.Assembling.Global.Sources;
using Application.FEM._2D.Assembling.Local;
using Application.FEM._2D.BasisFunctions;
using Application.FEM._2D.Grid;
using Application.FEM.Assembling._2D.Boundaries.First;
using Application.FEM.Assembling.PortraitBuilders;
using Application.FEM.Core.Assembling.Boundaries.First;
using Application.FEM.Core.Assembling.Inserters;
using Application.FEM.Core.Grid.Splitting;
using Application.InverseProblem;
using Application.InverseProblem._2D.Harmonic;
using Application.InverseProblem.Assembling.Concurrent;
using Application.InverseProblem.Assembling.Derivatives._2D.Harmonic;
using Application.InverseProblem.Assembling.Harmonic;
using Application.InverseProblem.Assembling.Regularization.Alpha._2D;
using Application.InverseProblem.Parameters;
using DirectProblem.IO;
using DirectProblem.TwoDimensional;
using DirectProblem.TwoDimensional.Assembling.Local;
using Domain;
using Domain.Boundaries;
using Domain.Edges;
using Domain.Enums;
using Domain.Environment;
using Domain.Materials;
using Domain.Nodes;
using System.Diagnostics;
using System.Globalization;
using System.Numerics;
using Tests;

Thread.CurrentThread.CurrentCulture = CultureInfo.InvariantCulture;

var trueGridParameters = TestGridParameters.GetGridWith0Dot003125StepWithElementNearToWellWith8Materials();
var trueMaterials = TestMaterials.GetMaterialsForGridWith0Dot003125StepWith8Materials();
var frequencies = TestFrequencies.GetFourFrequencies();

const int sourcePower = 1;

var (sources, receiverLines) = TestReceiverAndSourceConstructions.GetTenConstructions(sourcePower);

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

    new() { Type = ParameterType.VerticalBound, Index = 2, InitialValue = 0.5 },
    //new() { Type = ParameterType.VerticalBound, Index = 3, InitialValue = 1.5 },

    new() { Type = ParameterType.HorizontalBound, Index = 5, InitialValue = -2.5 },
    
};

const int maxPossibleThreads = 10;
var maxThreads = targetParameters.Length < maxPossibleThreads ? frequencies.Length : maxPossibleThreads;
var frequenciesParallelOptions = new ParallelOptions { MaxDegreeOfParallelism = frequencies.Length };
var parametersParallelOptions = new ParallelOptions { MaxDegreeOfParallelism = maxThreads };

var directProblems = new HarmonicRotorDirectProblem2DFactory().Create(maxThreads);
var measurementCalculatorManager = new HarmonicMeasurementCalculatorManager(directProblems);

var firstBoundaries =
    new ComplexDefinedValueFirstBoundaryProvider2D().GetOnAllBounds(trueGridParameters, new Complex(0, 0));

var trueGrid = measurementCalculatorManager.GetAllFreeCalculators().First().GenerateGrid(trueGridParameters);

Console.WriteLine("True measurements begin calculating");

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

        var potentialM = directProblemSolution.Get(receiverLines[j].ReceiverM);
        var potentialN = directProblemSolution.Get(receiverLines[j].ReceiverN);

        trueMeasurements[frequencyIndex, j] = (potentialM.Phase - potentialN.Phase) * 180d / Math.PI;

        Console.Write($"Frequency {frequencyIndex} source {j}                                   \r");
    }

    measurementCalculatorManager.ReleaseCalculator(measurementCalculator);
});

stopwatch.Stop();

var time = (double)stopwatch.ElapsedMilliseconds / 1000;

Console.WriteLine();
Console.WriteLine("True measurements calculated");
Console.WriteLine($"Elapsed time {time}");

//foreach (var phaseDifference in trueMeasurements)
//{
//    Console.WriteLine(phaseDifference);
//}

var gridParameters = TestGridParameters.GetGridWith0Dot003125StepWithElementNearToWellWith8Materials();
var materials = TestMaterials.GetMaterialsForGridWith0Dot003125StepWith8Materials();

var inverseProblemContextProvider = new HarmonicInverseProblem2DContextProvider();

var measurementDerivativesCalculators = directProblems
    .Select(p => new HarmonicMeasurementDerivativesCalculator2D(inverseProblemContextProvider, p)).ToArray<IHarmonicMeasurementDerivativesCalculator>();

var measurementDerivativesCalculatorManager = new HarmonicMeasurementDerivativeCalculatorManager(measurementDerivativesCalculators);
var equationAssembler = new ConcurrentHarmonicEquationAssembler2D(measurementDerivativesCalculatorManager, parametersParallelOptions);
var alphaRegularization = new AlphaRegularization2D
(
    inverseProblemContextProvider,
    new GaussElimination(),
    targetParameters,
    new Interval { Begin = 1e-3, End = 5 },
    2,
    1e-1,
    0.003125
);

var inverseProblem = new ConcurrentHarmonicInverseProblem2D
(
    inverseProblemContextProvider,
    measurementCalculatorManager,
    equationAssembler,
    alphaRegularization,
    new MinimizationMethodConfig(),
    parametersParallelOptions
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
