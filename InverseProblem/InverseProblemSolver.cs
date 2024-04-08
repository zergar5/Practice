using DirectProblem;
using DirectProblem.Core;
using DirectProblem.Core.Base;
using DirectProblem.Core.Global;
using DirectProblem.Core.GridComponents;
using DirectProblem.FEM;
using DirectProblem.GridGenerator;
using DirectProblem.GridGenerator.Intervals.Splitting;
using DirectProblem.SLAE;
using DirectProblem.TwoDimensional;
using DirectProblem.TwoDimensional.Assembling.Local;
using InverseProblem.Assembling;
using InverseProblem.Parameters;
using InverseProblem.SLAE;

namespace InverseProblem;

public class InverseProblemSolver
{
    private readonly GridBuilder2D _gridBuilder2D;
    private readonly DirectProblemSolver _directProblemSolver;
    private readonly SLAEAssembler _slaeAssembler;
    private readonly Regularizer _regularizer;
    private readonly GaussElimination _gaussElimination;
    private readonly LocalBasisFunctionsProvider _localBasisFunctionsProvider;

    private readonly ParametersCollection _parametersCollection;
    private readonly GridParameters _gridParameters;
    private readonly Source[] _sources;
    private readonly ReceiverLine[] _receiverLines;
    private readonly double[] _frequencies;
    private readonly Parameter[] _parameters;
    private readonly Vector _trueValues;
    private readonly double[,] _truePhaseDifferences;

    private double[,] _weightsSquares;
    private readonly double[,] _currentPhaseDifferences;
    private Grid<Node2D> _grid;
    private FEMSolution _femSolution;

    public InverseProblemSolver
    (
        GridBuilder2D gridBuilder2D,
        DirectProblemSolver directProblemSolver,
        SLAEAssembler slaeAssembler,
        Regularizer regularizer,
        GaussElimination gaussElimination,
        LocalBasisFunctionsProvider localBasisFunctionsProvider,
        ParametersCollection parametersCollection,
        GridParameters gridParameters,
        Source[] sources,
        ReceiverLine[] receiverLines,
        double[] frequencies,
        Parameter[] parameters,
        Vector trueValues,
        double[,] truePhaseDifferences
    )
    {
        _gridBuilder2D = gridBuilder2D;
        _directProblemSolver = directProblemSolver;
        _slaeAssembler = slaeAssembler;
        _regularizer = regularizer;
        _gaussElimination = gaussElimination;
        _localBasisFunctionsProvider = localBasisFunctionsProvider;
        _parametersCollection = parametersCollection;
        _gridParameters = gridParameters;
        _sources = sources;
        _receiverLines = receiverLines;
        _frequencies = frequencies;
        _parameters = parameters;
        _trueValues = trueValues;
        _truePhaseDifferences = truePhaseDifferences;

        CalculateWeightsSquares();

        _currentPhaseDifferences = new double[_frequencies.Length, _receiverLines.Length];
    }

    private void CalculateWeightsSquares()
    {
        _weightsSquares = new double[_frequencies.Length, _receiverLines.Length];

        for (var i = 0; i < _frequencies.Length; i++)
        {
            for (var j = 0; j < _receiverLines.Length; j++)
            {
                _weightsSquares[i, j] = Math.Pow(1 / _truePhaseDifferences[i, j], 2);
            }
        }

        _slaeAssembler.SetWeightsSquares(_weightsSquares);
    }

    public Vector Solve()
    {
        var functional = 1d;
        Equation<Matrix> equation = null!;

        RebuildGrid();
        _slaeAssembler.SetGrid(_grid);

        CalculatePhaseDifferences();

        for (var i = 1; i <= MethodsConfig.MaxIterations && functional > MethodsConfig.EpsDouble; i++)
        {
            equation = _slaeAssembler
                .SetCurrentPhaseDifferences(_currentPhaseDifferences)
                .BuildEquation();

            var regularizedEquation = _regularizer.Regularize(equation, _trueValues, out var alphas);

            var parametersDeltas = _gaussElimination.Solve(regularizedEquation);

            Vector.Sum(equation.Solution, parametersDeltas, equation.Solution);

            UpdateParameters(equation.Solution);

            CalculatePhaseDifferences();

            functional = CalculateFunctional(equation, alphas);

            CourseHolder.GetInfo(i, functional);
        }

        Console.WriteLine();

        return equation.Solution;
    }

    private void UpdateParameters(Vector parametersValues)
    {
        for (var i = 0; i < _parameters.Length; i++)
        {
            _parametersCollection.SetParameterValue(_parameters[i], parametersValues[i]);
        }
    }

    private double CalculateFunctional(Equation<Matrix> equation, double[] alphas)
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

        return functional;
    }

    private void RebuildGrid()
    {
        _grid = _gridBuilder2D
            .SetRAxis(new AxisSplitParameter(
                    _gridParameters.RControlPoints,
                    _gridParameters.RSplitters
                )
            )
            .SetZAxis(new AxisSplitParameter(
                    _gridParameters.ZControlPoints,
                    _gridParameters.ZSplitters
                )
            )
            .SetAreas(_gridParameters.Areas)
            .Build();

        _directProblemSolver.SetGrid(_grid);
    }

    private void ChangeMaterials()
    {
        _directProblemSolver.SetMaterials(_parametersCollection.Materials);
    }

    private void ChangeFrequency(double frequency)
    {
        _directProblemSolver.SetFrequency(frequency);
    }

    private void ChangeSource(Source source)
    {
        _directProblemSolver.SetSource(source);
    }

    private void CalculatePhaseDifferences()
    {
        //RebuildGrid();
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
            }
        }
    }

    private void SolveDirectProblem()
    {
        var solution = _directProblemSolver.Solve();

        _femSolution = new FEMSolution(_grid, solution, _localBasisFunctionsProvider);
    }
}