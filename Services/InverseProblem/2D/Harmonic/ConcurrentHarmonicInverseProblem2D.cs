using Application.DirectProblem._2D;
using Application.InverseProblem.Assembling.Concurrent;
using Application.InverseProblem.Assembling.Harmonic;
using Application.InverseProblem.Assembling.Regularization.Alpha;
using Application.InverseProblem.Parameters;
using Application.IO.Grid;
using Application.IO.Measurements;
using Application.MathObjects.Equation;
using Application.MathObjects.Matrices;
using Application.MathObjects.Vectors;
using Domain.Boundaries;
using Domain.Edges;
using Domain.Environment;
using Domain.Materials;
using Domain.Nodes;
using System.Numerics;
using static Application.FEM._2D.Grid.GridBuilder2D;

namespace Application.InverseProblem._2D.Harmonic;

public class ConcurrentHarmonicInverseProblem2D : IHarmonicInverseProblem<Grid2DParameters, MaterialWithSigmaMu, Node2D>
{
    private readonly IInverseProblemContextProvider<HarmonicInverseProblem2DContext> _problemContextProvider;
    private readonly IMeasurementCalculatorManager<IHarmonicDirectProblem2DWithSources<Complex, Grid2DParameters, MaterialWithSigmaMu>> _measurementCalculatorManager;
    private readonly IHarmonicEquationAssembler _equationAssembler;
    private readonly IAlphaRegularization _alphaRegularization;
    private readonly MinimizationMethodConfig _config;
    private readonly GridWriter2D _gridWriter;
    private readonly HarmonicMeasurementsWriter2D _measurementsWriter;
    private readonly ParallelOptions _parallelOptions;
    private readonly double _minGridStep;

    private Parameter[] _targetParameters;
    private double[,] _targetMeasurements;
    private double[,] _currentMeasurements;
    private double[,] _weights;

    public ConcurrentHarmonicInverseProblem2D
    (
        IInverseProblemContextProvider<HarmonicInverseProblem2DContext> problemContextProvider,
        IMeasurementCalculatorManager<IHarmonicDirectProblem2DWithSources<Complex, Grid2DParameters, MaterialWithSigmaMu>> measurementCalculatorManager,
        IHarmonicEquationAssembler equationAssembler,
        IAlphaRegularization alphaRegularization,
        MinimizationMethodConfig config,
        GridWriter2D gridWriter,
        HarmonicMeasurementsWriter2D measurementsWriter,
        ParallelOptions parallelOptions,
        double minGridStep
    )
    {
        _problemContextProvider = problemContextProvider;
        _measurementCalculatorManager = measurementCalculatorManager;
        _equationAssembler = equationAssembler;
        _alphaRegularization = alphaRegularization;
        _config = config;
        _gridWriter = gridWriter;
        _measurementsWriter = measurementsWriter;
        _parallelOptions = parallelOptions;
        _minGridStep = minGridStep;
    }

    public void SetGridParameters(Grid2DParameters gridParameters)
    {
        _problemContextProvider.Get().GridParameters = gridParameters;
    }

    public void SetMaterials(MaterialWithSigmaMu[] materials)
    {
        _problemContextProvider.Get().Materials = materials;
    }

    public void SetDefinedValueBoundaryCondition(IDefinedValueBoundaryCondition<Edge<Node2D>, Complex>[] definedValueBoundaryCondition)
    {
        _problemContextProvider.Get().DefinedValueBoundaryConditions = definedValueBoundaryCondition;

        var measurementsCalculators = _measurementCalculatorManager.GetAllFreeCalculators();

        foreach (var measurementsCalculator in measurementsCalculators)
        {
            measurementsCalculator.SetDefinedValueBoundaryCondition(definedValueBoundaryCondition);
        }
    }

    public void SetSourcesAndReceivers(Source<Node2D>[] sources, ReceiverLine<Node2D>[] receiverLines)
    {
        var problemContext = _problemContextProvider.Get();

        problemContext.ReceiverLines = receiverLines;
        problemContext.Sources = sources;
    }

    public void SetFrequencies(double[] frequencies)
    {
        _problemContextProvider.Get().Frequencies = frequencies;
    }

    public void SetTargetParametersAndMeasurements(Parameter[] parameters, double[,] targetMeasurements)
    {
        var problemContext = _problemContextProvider.Get();
        var gridParameters = problemContext.GridParameters;
        var materials = problemContext.Materials;
        var frequencies = problemContext.Frequencies;
        var receiverLines = problemContext.ReceiverLines;

        foreach (var parameter in parameters)
        {
            switch (parameter.Type)
            {
                case ParameterType.Sigma:
                    materials[parameter.Index].Sigma = parameter.InitialValue;
                    break;
                case ParameterType.VerticalBound:
                    gridParameters.XControlPoints[parameter.Index] = parameter.InitialValue;
                    break;
                case ParameterType.HorizontalBound:
                    gridParameters.YControlPoints[parameter.Index] = parameter.InitialValue;
                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }
        }

        _targetParameters = parameters;
        _targetMeasurements = targetMeasurements;
        _currentMeasurements = new double[frequencies.Length, receiverLines.Length];
        _weights = new double[frequencies.Length, receiverLines.Length];

        CalculateWeights();
    }

    public IVector<double> Solve()
    {
        var previousFunctional = double.MaxValue;
        var equation = _equationAssembler.AllocateEquation(_targetParameters, _targetMeasurements, _weights);

        GetMeasurements();

        var functional = CalculateFunctional();

        Console.WriteLine($"Initial values functional {functional:E6}");

        var problemContext = _problemContextProvider.Get();
        var gridParameters = problemContext.GridParameters;
        var materials = problemContext.Materials;
        var receiverLines = problemContext.ReceiverLines;
        var frequencies = problemContext.Frequencies;

        _gridWriter.WriteAreas(gridParameters, materials, "initial areas.txt");
        _measurementsWriter.WriteMeasurements(receiverLines, _currentMeasurements, frequencies, functional, "initial measurements.txt");

        for (var i = 1; i <= _config.MaxIterations && CheckFunctional(functional, previousFunctional); i++)
        {
            equation = _equationAssembler.Assemble(_currentMeasurements);

            var parameterDeltas = _alphaRegularization.Regularize(equation);

            equation.Solution.Sum(parameterDeltas, equation.Solution);

            UpdateTargetParameterValues(equation.Solution);
            GetMeasurements();

            previousFunctional = functional;
            functional = CalculateFunctional();

            Console.WriteLine($"Iteration {i} functional {functional:E6}");

            for (var j = 0; j < equation.Solution.Count; j++)
            {
                Console.WriteLine($"Parameter {_targetParameters[j].Type} {_targetParameters[j].Index} value {equation.Solution[j]:F16} delta {parameterDeltas[j]:F16}");
            }

            _gridWriter.WriteAreas(gridParameters, materials, $"iteration {i} areas.txt");
            _measurementsWriter.WriteMeasurements(receiverLines, _currentMeasurements, frequencies, functional, $"iteration {i} measurements.txt");
        }

        return equation.Solution;
    }

    private void CalculateWeights()
    {
        var problemContext = _problemContextProvider.Get();

        for (var i = 0; i < problemContext.Frequencies.Length; i++)
        {
            for (var j = 0; j < problemContext.ReceiverLines.Length; j++)
            {
                _weights[i, j] = 1 / _targetMeasurements[i, j];
            }
        }
    }

    private void UpdateTargetParameterValues(IVector<double> parameterValues)
    {
        var problemContext = _problemContextProvider.Get();

        for (var i = 0; i < _targetParameters.Length; i++)
        {
            var parameter = _targetParameters[i];

            switch (parameter.Type)
            {
                case ParameterType.Sigma:
                    problemContext.Materials[parameter.Index].Sigma = parameterValues[i];
                    break;
                case ParameterType.VerticalBound:
                    parameterValues[i] = RoundToGridStep(parameterValues[i], _minGridStep);
                    problemContext.GridParameters.XControlPoints[parameter.Index] = parameterValues[i];
                    break;
                case ParameterType.HorizontalBound:
                    parameterValues[i] = RoundToGridStep(parameterValues[i], _minGridStep);
                    problemContext.GridParameters.YControlPoints[parameter.Index] = parameterValues[i];
                    break;
                default:
                    throw new NotSupportedException();
            }
        }
    }

    private void GetMeasurements()
    {
        var problemContext = _problemContextProvider.Get();
        var frequencies = problemContext.Frequencies;
        var receiverLines = problemContext.ReceiverLines;
        var sources = problemContext.Sources;

        problemContext.Grid = _measurementCalculatorManager.GetAllFreeCalculators().First().GenerateGrid(problemContext.GridParameters);

        Console.WriteLine("Measurements begin calculating");

        Parallel.For(0, frequencies.Length, _parallelOptions, frequencyIndex =>
        {
            var measurementCalculator = _measurementCalculatorManager.WaitFreeCalculator();

            measurementCalculator.SetGrid(problemContext.Grid);
            measurementCalculator.SetMaterials(problemContext.Materials);
            measurementCalculator.SetFrequency(frequencies[frequencyIndex]);

            for (var j = 0; j < receiverLines.Length; j++)
            {
                measurementCalculator.SetSources([sources[j]]);

                var solution = measurementCalculator.Solve();

                var fieldM = solution.Get(receiverLines[j].ReceiverM);
                var fieldN = solution.Get(receiverLines[j].ReceiverN);

                _currentMeasurements[frequencyIndex, j] = (fieldM.Phase - fieldN.Phase) * 180d / Math.PI;

                Console.Write($"Frequency {frequencyIndex} source {j}                                   \r");
            }

            _measurementCalculatorManager.ReleaseCalculator(measurementCalculator);
        });

        Console.WriteLine();
        Console.WriteLine("Measurements calculated");

        //foreach (var phaseDifference in _currentMeasurements)
        //{
        //    Console.WriteLine(phaseDifference);
        //}
    }

    private double CalculateFunctional()
    {
        var problemContext = _problemContextProvider.Get();
        var frequencies = problemContext.Frequencies;
        var receiverLines = problemContext.ReceiverLines;
        var functional = 0d;

        for (var i = 0; i < frequencies.Length; i++)
        {
            for (var j = 0; j < receiverLines.Length; j++)
            {
                functional += Math.Pow(_weights[i, j] * (_currentMeasurements[i, j] - _targetMeasurements[i, j]), 2);
            }
        }

        functional = Math.Sqrt(functional) / (frequencies.Length * receiverLines.Length);

        return functional;
    }

    private bool CheckFunctional(double currentFunctional, double previousFunctional)
    {
        var functionalRatio = Math.Abs(currentFunctional / previousFunctional);

        return Math.Abs(double.Max(1 / functionalRatio, functionalRatio) - 1) >= _config.MinimizePrecision &&
               currentFunctional >= _config.MinimizePrecision;
    }

    private double RoundToGridStep(double parameterValue, double step)
    {
        return Math.Round(parameterValue / step) * step;
    }
}