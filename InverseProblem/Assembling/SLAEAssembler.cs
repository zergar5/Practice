using DirectProblem;
using DirectProblem.Core;
using DirectProblem.Core.Base;
using DirectProblem.Core.Boundary;
using DirectProblem.Core.Global;
using DirectProblem.Core.GridComponents;
using DirectProblem.FEM;
using DirectProblem.GridGenerator;
using DirectProblem.GridGenerator.Intervals.Splitting;
using DirectProblem.IO;
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
    private readonly double[] _omegas;
    private readonly Parameter[] _parameters;
    private readonly double[,] _truePotentialDifferences;
    private readonly double[,] _weightsSquares;
    private readonly double[,] _potentialDifferences;
    private readonly double[,,] _derivativesPotentialDifferences;
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
        double[] omegas,
        Parameter[] parameters,
        Vector initialValues,
        double[,] truePotentialDifferences
    )
    {
        _gridBuilder2D = gridBuilder2D;
        _directProblemSolver = directProblemSolver;
        _localBasisFunctionsProvider = localBasisFunctionsProvider;

        _parametersCollection = parametersCollection;
        _gridParameters = gridParameters;
        _sources = sourcesLine;
        _receiverLines = receiversLines;
        _omegas = omegas;
        _parameters = parameters;
        _truePotentialDifferences = truePotentialDifferences;

        _equation = new Equation<Matrix>(new Matrix(_parameters.Length), initialValues,
            new Vector(_parameters.Length));

        _weightsSquares = new double[_omegas.Length, _truePotentialDifferences.Length];
        CalculateWeightsSquares();

        _potentialDifferences = new double[_omegas.Length, _receiverLines.Length];

        _derivativesPotentialDifferences = new double[_parameters.Length, _omegas.Length, _receiverLines.Length];
    }

    public Equation<Matrix> BuildEquation()
    {
        CalculatePotentialDifferences();
        AssembleMatrix();
        AssembleRightPart();

        return _equation;
    }

    private void CalculateWeightsSquares()
    {
        for (var i = 0; i < _omegas.Length; i++)
        {
            for (var k = 0; k < _receiverLines.Length; k++)
            {
                _weightsSquares[i, k] = Math.Pow(1d / _truePotentialDifferences[i, k], 2);
            }
        }
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
                
                for (var i = 0; i < _omegas.Length; i++)
                {
                    for (var k = 0; k < _receiverLines.Length; k++)
                    {
                        sum += _weightsSquares[i, k] * _derivativesPotentialDifferences[q, i, k] *
                                                  _derivativesPotentialDifferences[s, i, k];
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
            
            for (var i = 0; i < _omegas.Length; i++)
            {
                for (var k = 0; k < _receiverLines.Length; k++)
                {
                    sum -= _weightsSquares[i, k] *
                           (_potentialDifferences[i, k] - _truePotentialDifferences[i, k]) *
                           _derivativesPotentialDifferences[q, i, k];
                }
            }

            _equation.RightPart[q] = sum;
        }
    }

    private void CalculatePotentialDifferences()
    {
        CalculateOriginalPotentialDifferences();

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

            CalculatePotentialDifferencesDerivatives(k, delta);

            _parametersCollection.SetParameterValue(_parameters[k], parameterValue);
        }
    }

    private void CalculateOriginalPotentialDifferences()
    {
        RebuildGrid();
        ChangeMaterials();

        for (var i = 0; i < _omegas.Length; i++)
        {
            ChangeFrequency(_omegas[i]);

            for (var j = 0; j < _sources.Length; j++)
            {
                ChangeSource(_sources[j]);
                SolveDirectProblem();

                var potentialM = _femSolution.Calculate(_receiverLines[i].PointM);
                var potentialN = _femSolution.Calculate(_receiverLines[i].PointN);

                _potentialDifferences[i, j] = (potentialM.Phase - potentialN.Phase) * 180d / Math.PI;
            }
        }
    }

    private void CalculatePotentialDifferencesDerivatives(int parameterIndex, double delta)
    {
        for (var i = 0; i < _omegas.Length; i++)
        {
            ChangeFrequency(_omegas[i]);

            for (var j = 0; j < _sources.Length; j++)
            {
                ChangeSource(_sources[j]);
                SolveDirectProblem();

                var potentialM = _femSolution.Calculate(_receiverLines[i].PointM);
                var potentialN = _femSolution.Calculate(_receiverLines[i].PointN);

                _potentialDifferences[i, j] = (potentialM.Phase - potentialN.Phase) * 180d / Math.PI;

                _derivativesPotentialDifferences[parameterIndex, i, j] =
                    (_derivativesPotentialDifferences[parameterIndex, i, j] - _potentialDifferences[i, j]) / delta;
            }
        }
    }
}