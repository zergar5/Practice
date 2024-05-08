using DirectProblem;
using DirectProblem.Core;
using DirectProblem.Core.Base;
using DirectProblem.Core.Global;
using DirectProblem.Core.GridComponents;
using DirectProblem.FEM;
using DirectProblem.GridGenerator;
using DirectProblem.GridGenerator.Intervals.Splitting;
using DirectProblem.IO;
using DirectProblem.SLAE;
using DirectProblem.TwoDimensional;
using DirectProblem.TwoDimensional.Assembling.Local;
using InverseProblem.Assembling;
using InverseProblem.Parameters;
using InverseProblem.SLAE;

namespace InverseProblem;

public class InverseProblemSolver
{
    private readonly DirectProblemSolver[] _directProblemSolver;
    private readonly SLAEAssembler _slaeAssembler;
    private readonly Regularizer _regularizer;
    private readonly GaussElimination _gaussElimination;
    private readonly LocalBasisFunctionsProvider[] _localBasisFunctionsProvider;

    private readonly ParametersCollection[] _parametersCollection;
    private readonly Source[] _sources;
    private readonly ReceiverLine[] _receiverLines;
    private readonly double[] _frequencies;
    private readonly Parameter[] _parameters;
    private readonly double[,] _truePhaseDifferences;
    private readonly Vector _initialValues;

    private double[,] _weightsSquares;
    private readonly double[,] _currentPhaseDifferences;
    private readonly Grid<Node2D> _grid;
    private FEMSolution _femSolution;

    public InverseProblemSolver
    (
        DirectProblemSolver[] directProblemSolver,
        SLAEAssembler slaeAssembler,
        Regularizer regularizer,
        GaussElimination gaussElimination,
        LocalBasisFunctionsProvider[] localBasisFunctionsProvider,
        Grid<Node2D> grid,
        ParametersCollection[] parametersCollection,
        Source[] sources,
        ReceiverLine[] receiverLines,
        double[] frequencies,
        Parameter[] parameters,
        double[,] truePhaseDifferences,
        Vector initialValues
    )
    {
        _directProblemSolver = directProblemSolver;
        _slaeAssembler = slaeAssembler;
        _regularizer = regularizer;
        _gaussElimination = gaussElimination;
        _localBasisFunctionsProvider = localBasisFunctionsProvider;
        _grid = grid;
        _parametersCollection = parametersCollection;
        _sources = sources;
        _receiverLines = receiverLines;
        _frequencies = frequencies;
        _parameters = parameters;
        _truePhaseDifferences = truePhaseDifferences;
        _initialValues = initialValues;

        CalculateWeightsSquares();

        _currentPhaseDifferences = new double[_frequencies.Length, _receiverLines.Length];
    }

    private void CalculateWeightsSquares()
    {
        _weightsSquares = new double[_frequencies.Length, _receiverLines.Length];

        Parallel.For(0, _frequencies.Length, i =>
        {
            for (var j = 0; j < _receiverLines.Length; j++)
            {
                _weightsSquares[i, j] = Math.Pow(1 / _truePhaseDifferences[i, j], 2);
            }
        });

        _slaeAssembler.SetWeightsSquares(_weightsSquares);
    }

    public Vector Solve()
    {
        var previousFunctional = 2d;
        var functional = 1d;
        Equation<Matrix> equation = null!;

        _slaeAssembler.SetGrid(_grid);

        var resultO = new ResultIO("../InverseProblem/Results/8OtherSigmasCloseAndNearToWell/");
        var gridO = new GridIO("../InverseProblem/Results/8OtherSigmasCloseAndNearToWell/");

        CalculatePhaseDifferences();
        resultO.WriteInverseProblemIteration(_receiverLines, _currentPhaseDifferences, _frequencies, "iteration 0 phase differences.txt");
        gridO.WriteAreas(_grid, _initialValues, "iteration 0 areas.txt");

        for (var i = 1; i <= MethodsConfig.MaxIterations && CheckFunctional(functional, previousFunctional); i++)
        {
            equation = _slaeAssembler
                .SetCurrentPhaseDifferences(_currentPhaseDifferences)
                .BuildEquation();

            var regularizedEquation = _regularizer.Regularize(equation, out var alphas);

            var parametersDeltas = _gaussElimination.Solve(regularizedEquation);

            Vector.Sum(equation.Solution, parametersDeltas, equation.Solution);

            UpdateParameters(equation.Solution);

            CalculatePhaseDifferences();

            previousFunctional = functional;

            functional = CalculateFunctional();

            CourseHolder.GetFunctionalInfo(i, functional);

            Console.WriteLine();

            for (var j = 0; j < equation.Solution.Count; j++)
            {
                Console.WriteLine($"{equation.Solution[j]} {parametersDeltas[j]} {alphas[j]}");
            }

            resultO.WriteInverseProblemIteration(_receiverLines, _currentPhaseDifferences, _frequencies, $"iteration {i} phase differences.txt");
            gridO.WriteAreas(_grid, equation.Solution, $"iteration {i} areas.txt");
        }

        Console.WriteLine();

        return equation.Solution;
    }

    private void UpdateParameters(Vector parametersValues)
    {
        for (var i = 0; i < _parameters.Length; i++)
        {
            foreach (var parametersCollection in _parametersCollection)
            {
                parametersCollection.SetParameterValue(_parameters[i], parametersValues[i]);
            }
        }
    }

    private double CalculateFunctional()
    {
        var functional = 0d;

        for (var i = 0; i < _frequencies.Length; i++)
        {
            for (var j = 0; j < _receiverLines.Length; j++)
            {
                functional += _weightsSquares[i, j] *
                              Math.Pow(_currentPhaseDifferences[i, j] - _truePhaseDifferences[i, j], 2);
            }
        }

        functional = Math.Sqrt(functional) / (_frequencies.Length * _receiverLines.Length);

        return functional;
    }

    private bool CheckFunctional(double currentFunctional, double previousFunctional)
    {
        var functionalRatio = Math.Abs(currentFunctional / previousFunctional);
        return Math.Abs(double.Max(1 / functionalRatio, functionalRatio) - 1) > 5e-1 &&
               currentFunctional >= MethodsConfig.FunctionalPrecision;
    }

    private void ChangeMaterials()
    {
        _directProblemSolver[0].SetMaterials(_parametersCollection[0].Materials);
    }

    private void ChangeFrequency(double frequency)
    {
        _directProblemSolver[0].SetFrequency(frequency);
    }

    private void ChangeSource(Source source)
    {
        _directProblemSolver[0].SetSource(source);
    }

    private void CalculatePhaseDifferences()
    {
        ChangeMaterials();

        for (var i = 0; i < _frequencies.Length; i++)
        {
            ChangeFrequency(_frequencies[i]);

            for (var j = 0; j < _receiverLines.Length; j++)
            {
                ChangeSource(_sources[j]);
                SolveDirectProblem();

                var fieldM = _femSolution.Calculate(_receiverLines[j].PointM);
                var fieldN = _femSolution.Calculate(_receiverLines[j].PointN);

                _currentPhaseDifferences[i, j] = (fieldM.Phase - fieldN.Phase) * 180d / Math.PI;

                Console.Write($"original frequency {i} source {j}                          \r");
            }
        }
    }

    private void SolveDirectProblem()
    {
        var solution = _directProblemSolver[0].AssembleSLAE().Solve();

        _femSolution = new FEMSolution(_grid, solution, _localBasisFunctionsProvider[0]);
    }
}