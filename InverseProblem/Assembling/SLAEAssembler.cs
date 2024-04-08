using DirectProblem;
using DirectProblem.Core;
using DirectProblem.Core.Base;
using DirectProblem.Core.Global;
using DirectProblem.Core.GridComponents;
using DirectProblem.GridGenerator;
using DirectProblem.GridGenerator.Intervals.Splitting;
using DirectProblem.TwoDimensional;
using DirectProblem.TwoDimensional.Assembling.Local;
using InverseProblem.Parameters;
using Vector = DirectProblem.Core.Base.Vector;

namespace InverseProblem.Assembling;

public class SLAEAssembler
{
    private readonly GridBuilder2D _gridBuilder2D;
    private readonly DirectProblemSolver _directProblemSolver;
    private readonly LocalBasisFunctionsProvider _localBasisFunctionsProvider;

    private readonly ParametersCollection _parametersCollection;
    private readonly GridParameters _gridParameters;
    private readonly Source[] _sources;
    private readonly ReceiverLine[] _receiverLines;
    private readonly double[] _frequencies;
    private readonly Parameter[] _parameters;
    private readonly double[,] _truePhaseDifferences;
    private double[,] _weightsSquares;
    private double[,] _phaseDifferences;
    private readonly double[,,] _phaseDifferencesDerivatives;
    private readonly Equation<Matrix> _equation;

    private Grid<Node2D> _grid;
    private FEMSolution _femSolution;

    public SLAEAssembler
    (
        GridBuilder2D gridBuilder2D,
        DirectProblemSolver directProblemSolver,
        LocalBasisFunctionsProvider localBasisFunctionsProvider,
        ParametersCollection parametersCollection,
        GridParameters gridParameters,
        Source[] sourcesLine,
        ReceiverLine[] receiversLines,
        double[] frequencies,
        Parameter[] parameters,
        Vector initialValues,
        double[,] truePhaseDifferences,
        double[,] weightsSquares
    )
    {
        _gridBuilder2D = gridBuilder2D;
        _directProblemSolver = directProblemSolver;
        _localBasisFunctionsProvider = localBasisFunctionsProvider;

        _parametersCollection = parametersCollection;
        _gridParameters = gridParameters;
        _sources = sourcesLine;
        _receiverLines = receiversLines;
        _frequencies = frequencies;
        _parameters = parameters;
        _truePhaseDifferences = truePhaseDifferences;
        _weightsSquares = weightsSquares;

        _equation = new Equation<Matrix>(new Matrix(_parameters.Length), initialValues,
            new Vector(_parameters.Length));

        _phaseDifferences = new double[_frequencies.Length, _receiverLines.Length];

        _phaseDifferencesDerivatives = new double[_parameters.Length, _frequencies.Length, _receiverLines.Length];
    }

    public SLAEAssembler SetWeightsSquares(double[,] weightsSquares)
    {
        _weightsSquares = weightsSquares;

        return this;
    }

    public SLAEAssembler SetCurrentPhaseDifferences(double[,] phaseDifferences)
    {
        _phaseDifferences = phaseDifferences;

        return this;
    }

    public Equation<Matrix> BuildEquation()
    {
        CalculatePhaseDifferences();
        AssembleMatrix();
        AssembleRightPart();

        return _equation;
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

    private void SolveDirectProblem()
    {
        var solution = _directProblemSolver.Solve();

        _femSolution = new FEMSolution(_grid, solution, _localBasisFunctionsProvider);
    }

    private void AssembleMatrix()
    {
        for (var q = 0; q < _equation.Matrix.CountRows; q++)
        {
            for (var s = 0; s < _equation.Matrix.CountColumns; s++)
            {
                var sum = 0d;

                for (var i = 0; i < _frequencies.Length; i++)
                {
                    for (var k = 0; k < _receiverLines.Length; k++)
                    {
                        sum += _weightsSquares[i, k] * _phaseDifferencesDerivatives[q, i, k] *
                                                  _phaseDifferencesDerivatives[s, i, k];
                    }
                }

                _equation.Matrix[q, s] = sum;
            }
        }
    }

    private void AssembleRightPart()
    {
        for (var q = 0; q < _equation.Matrix.CountRows; q++)
        {
            var sum = 0d;

            for (var i = 0; i < _frequencies.Length; i++)
            {
                for (var k = 0; k < _receiverLines.Length; k++)
                {
                    sum -= _weightsSquares[i, k] *
                           (_phaseDifferences[i, k] - _truePhaseDifferences[i, k]) *
                           _phaseDifferencesDerivatives[q, i, k];
                }
            }

            _equation.RightPart[q] = sum;
        }
    }

    private void CalculatePhaseDifferences()
    {
        for (var k = 0; k < _parameters.Length; k++)
        {
            switch (_parameters[k].ParameterType)
            {
                case ParameterType.Sigma:
                    ChangeMaterials();
                    break;
                case ParameterType.VerticalBound:
                case ParameterType.HorizontalBound:
                    RebuildGrid();
                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }

            var parameterValue = _parametersCollection.GetParameterValue(_parameters[k]);
            var delta = parameterValue * 1e-3;
            _parametersCollection.SetParameterValue(_parameters[k], parameterValue + delta);

            CalculatePhaseDifferencesDerivatives(k, delta);

            _parametersCollection.SetParameterValue(_parameters[k], parameterValue);
        }
    }

    private void CalculatePhaseDifferencesDerivatives(int parameterIndex, double delta)
    {
        for (var i = 0; i < _frequencies.Length; i++)
        {
            ChangeFrequency(_frequencies[i]);

            for (var j = 0; j < _receiverLines.Length; j++)
            {
                ChangeSource(_sources[j]);
                SolveDirectProblem();

                var fieldM = _femSolution.Calculate(_receiverLines[i].PointM);
                var fieldN = _femSolution.Calculate(_receiverLines[i].PointN);

                _phaseDifferences[i, j] = (fieldM.Phase - fieldN.Phase) * 180d / Math.PI;

                _phaseDifferencesDerivatives[parameterIndex, i, j] =
                    (_phaseDifferencesDerivatives[parameterIndex, i, j] - _phaseDifferences[i, j]) / delta;
            }
        }
    }
}