using DirectProblem;
using DirectProblem.Core;
using DirectProblem.Core.Base;
using DirectProblem.Core.Global;
using DirectProblem.Core.GridComponents;
using DirectProblem.TwoDimensional;
using DirectProblem.TwoDimensional.Assembling.Local;
using InverseProblem.Parameters;
using Vector = DirectProblem.Core.Base.Vector;

namespace InverseProblem.Assembling;

public class SLAEAssembler
{
    private readonly DirectProblemSolver[] _directProblemSolver;
    private readonly LocalBasisFunctionsProvider[] _localBasisFunctionsProvider;

    private readonly ParametersCollection[] _parametersCollection;
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

    private readonly Task[] _tasks;

    public SLAEAssembler
    (
        DirectProblemSolver[] directProblemSolver,
        LocalBasisFunctionsProvider[] localBasisFunctionsProvider,
        ParametersCollection[] parametersCollection,
        Source[] sourcesLine,
        ReceiverLine[] receiversLines,
        double[] frequencies,
        Parameter[] parameters,
        Vector initialValues,
        double[,] truePhaseDifferences
    )
    {
        _directProblemSolver = directProblemSolver;
        _localBasisFunctionsProvider = localBasisFunctionsProvider;

        _parametersCollection = parametersCollection;
        _sources = sourcesLine;
        _receiverLines = receiversLines;
        _frequencies = frequencies;
        _parameters = parameters;
        _truePhaseDifferences = truePhaseDifferences;

        _equation = new Equation<Matrix>(new Matrix(_parameters.Length), initialValues,
            new Vector(_parameters.Length));

        _phaseDifferences = new double[_frequencies.Length, _receiverLines.Length];

        _phaseDifferencesDerivatives = new double[_parameters.Length, _frequencies.Length, _receiverLines.Length];

        _tasks = new Task[directProblemSolver.Length];
    }

    public SLAEAssembler SetGrid(Grid<Node2D> grid)
    {
        _grid = grid;

        return this;
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

    private void ChangeMaterials(int solverId)
    {
        _directProblemSolver[solverId].SetMaterials(_parametersCollection[solverId].Materials);
    }

    private void ChangeFrequency(double frequency, int solverId)
    {
        _directProblemSolver[solverId].SetFrequency(frequency);
    }

    private void ChangeSource(Source source, int solverId)
    {
        _directProblemSolver[solverId].SetSource(source);
    }

    private FEMSolution SolveDirectProblem(int solverId)
    {
        var solution = _directProblemSolver[solverId].AssembleSLAE().Solve();

        return new FEMSolution(_grid, solution, _localBasisFunctionsProvider[solverId]);
    }

    private void AssembleMatrix()
    {
        Parallel.For(0, _equation.Matrix.CountRows, q =>
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
        });
    }

    private void AssembleRightPart()
    {
        Parallel.For(0, _equation.Matrix.CountRows, q =>
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
        });
    }

    private void CalculatePhaseDifferences()
    {
        for (var i = 0; i < _parameters.Length; i++)
        {
            var taskId = i % _tasks.Length;

            if (i >= _tasks.Length)
            {
                taskId = Task.WaitAny(_tasks);
            }

            var task = new Task(j =>
            {
                var parameterValue = _parametersCollection[taskId].GetParameterValue(_parameters[(int)j]);
                var delta = parameterValue * 1e-1;
                _parametersCollection[taskId].SetParameterValue(_parameters[(int)j], parameterValue + delta);

                ChangeMaterials(taskId);

                CalculatePhaseDifferencesDerivatives((int)j, delta, taskId);

                _parametersCollection[taskId].SetParameterValue(_parameters[(int)j], parameterValue);
            }, i);

            task.Start();

            _tasks[taskId] = task;
        }

        Task.WaitAll(_tasks);
    }

    private void CalculatePhaseDifferencesDerivatives(int parameterIndex, double delta, int solverId)
    {
        for (var i = 0; i < _frequencies.Length; i++)
        {
            ChangeFrequency(_frequencies[i], solverId);

            for (var j = 0; j < _receiverLines.Length; j++)
            {
                ChangeSource(_sources[j], solverId);
                var femSolution = SolveDirectProblem(solverId);

                var fieldM = femSolution.Calculate(_receiverLines[j].PointM);
                var fieldN = femSolution.Calculate(_receiverLines[j].PointN);

                _phaseDifferencesDerivatives[parameterIndex, i, j] = (fieldM.Phase - fieldN.Phase) * 180d / Math.PI;

                _phaseDifferencesDerivatives[parameterIndex, i, j] =
                    (_phaseDifferencesDerivatives[parameterIndex, i, j] - _phaseDifferences[i, j]) / delta;

                Console.Write($"derivative {parameterIndex} frequency {i} source {j}                           \r");
            }
        }
    }
}