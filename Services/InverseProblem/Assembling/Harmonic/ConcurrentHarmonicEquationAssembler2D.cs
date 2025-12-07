using Application.InverseProblem.Assembling.Concurrent;
using Application.InverseProblem.Assembling.Derivatives._2D.Harmonic;
using Application.InverseProblem.Parameters;
using Application.MathObjects.Equation;
using Application.MathObjects.Matrices;
using Application.MathObjects.Vectors;

namespace Application.InverseProblem.Assembling.Harmonic;

public class ConcurrentHarmonicEquationAssembler2D : IHarmonicEquationAssembler
{
    private readonly IMeasurementCalculatorManager<IHarmonicMeasurementDerivativesCalculator> _measurementCalculatorManager;
    private readonly ParallelOptions _parallelOptions;

    private Parameter[] _parameters;
    private double[,] _targetMeasurements;
    private double[,] _weightsSquares;
    private double[,,] _measurementDerivatives;
    private IEquation<IMatrix<double>, double> _equation;

    public ConcurrentHarmonicEquationAssembler2D
    (
        IMeasurementCalculatorManager<IHarmonicMeasurementDerivativesCalculator> measurementCalculatorManager,
        ParallelOptions parallelOptions
    )
    {
        _measurementCalculatorManager = measurementCalculatorManager;
        _parallelOptions = parallelOptions;
    }

    public IHarmonicEquationAssembler AllocateEquation(Parameter[] parameters, double[,] targetMeasurements, double[,] weightSquares)
    {
        _parameters = parameters;
        _targetMeasurements = targetMeasurements;
        _weightsSquares = weightSquares;
        _measurementDerivatives = new double[parameters.Length, targetMeasurements.GetLength(0), targetMeasurements.GetLength(1)];

        _equation = new Equation<IMatrix<double>, double>
        (
            new Matrix<double>(parameters.Length),
            new Vector<double>(parameters.Select(p => p.InitialValue).ToArray()),
            new Vector<double>(parameters.Length)
        );

        var calculators = _measurementCalculatorManager.GetAllFreeCalculators();

        foreach (var calculator in calculators)
        {
            calculator.Init();
        }

        return this;
    }

    public IEquation<IMatrix<double>, double> Assemble(double[,] measurements)
    {
        CalculateMeasurementDerivatives(measurements);

        //foreach (var phaseDifference in _measurementDerivatives)
        //{
        //    Console.WriteLine(phaseDifference);
        //}

        AssembleMatrix(_measurementDerivatives);
        AssembleRightPart(measurements, _measurementDerivatives);

        return _equation;
    }

    private void CalculateMeasurementDerivatives(double[,] measurements)
    {
        Console.WriteLine("Measurements derivatives begin calculating");

        Parallel.For(0, _parameters.Length, _parallelOptions, parameterIndex =>
        {
            var measurementDerivativesCalculator = _measurementCalculatorManager.WaitFreeCalculator();

            var measurementDerivatives =
                measurementDerivativesCalculator.CalcDerivativesForParameter(_parameters[parameterIndex], measurements);

            for (var i = 0; i < measurementDerivatives.GetLength(0); i++)
            {
                for (var j = 0; j < measurementDerivatives.GetLength(1); j++)
                {
                    _measurementDerivatives[parameterIndex, i, j] = measurementDerivatives[i, j];
                }
            }

            _measurementCalculatorManager.ReleaseCalculator(measurementDerivativesCalculator);
        });

        Console.WriteLine();
        Console.WriteLine("Measurements derivatives calculated");
    }

    private void AssembleMatrix(double[,,] measurementDerivatives)
    {
        Parallel.For(0, _equation.Matrix.RowCount, _parallelOptions, q =>
        {
            for (var s = 0; s < _equation.Matrix.ColumnCount; s++)
            {
                var sum = 0d;

                for (var i = 0; i < measurementDerivatives.GetLength(1); i++)
                {
                    for (var k = 0; k < measurementDerivatives.GetLength(2); k++)
                    {
                        sum += _weightsSquares[i, k] * measurementDerivatives[q, i, k] *
                               measurementDerivatives[s, i, k];
                    }
                }

                _equation.Matrix[q, s] = sum;
            }
        });
    }

    private void AssembleRightPart(double[,] measurements, double[,,] measurementDerivatives)
    {
        Parallel.For(0, _equation.Matrix.RowCount, _parallelOptions, q =>
        {
            var sum = 0d;

            for (var i = 0; i < measurements.GetLength(0); i++)
            {
                for (var k = 0; k < measurements.GetLength(1); k++)
                {
                    sum -= _weightsSquares[i, k] *
                           (measurements[i, k] - _targetMeasurements[i, k]) *
                           measurementDerivatives[q, i, k];
                }
            }

            _equation.RightPart[q] = sum;
        });
    }
}