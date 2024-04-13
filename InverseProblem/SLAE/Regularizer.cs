using DirectProblem.Core.Base;
using DirectProblem.Core.Global;
using InverseProblem.Parameters;

namespace InverseProblem.SLAE;

public class Regularizer
{
    private readonly GaussElimination _gaussElimination;
    private readonly double[] _alphas;
    private readonly Equation<Matrix> _regularizedEquation;
    private readonly Vector _previousDeltas;

    public Regularizer(GaussElimination gaussElimination, Parameter[] parameters)
    {
        _gaussElimination = gaussElimination;
        _alphas = new double[parameters.Length];
        _regularizedEquation = new Equation<Matrix>(
            new Matrix(parameters.Length),
            new Vector(parameters.Length),
            new Vector(parameters.Length)
            );
        _previousDeltas = new Vector(parameters.Length);
    }

    public Equation<Matrix> Regularize(Equation<Matrix> equation, Vector trueParametersValues, out double[] alphas)
    {
        alphas = SetupAlphas(equation.Matrix);

        alphas = FindPossibleAlphas(equation, alphas, trueParametersValues);

        alphas = FindBestAlphas(equation, alphas, trueParametersValues);

        AssembleSLAE(equation, alphas, trueParametersValues);

        return _regularizedEquation;
    }

    private double[] SetupAlphas(Matrix matrix)
    {
        for (var i = 0; i < matrix.CountRows; i++)
        {
            _alphas[i] = matrix[i, i] * 1e-8;
        }

        return _alphas;
    }

    private void AssembleSLAE(Equation<Matrix> equation, double[] alphas, Vector trueParametersValues)
    {
        equation.Matrix.Copy(_regularizedEquation.Matrix);
        Matrix.SumToDiagonal(equation.Matrix, alphas, _regularizedEquation.Matrix);

        equation.RightPart.Copy(_regularizedEquation.RightPart);

        //Vector.Subtract
        //(
        //    equation.RightPart,
        //    Vector.Multiply
        //    (
        //        alphas,
        //        Vector.Subtract
        //        (
        //            equation.Solution,
        //            trueParametersValues,
        //            _regularizedEquation.RightPart
        //            ),
        //        _regularizedEquation.RightPart
        //        ),
        //    _regularizedEquation.RightPart
        //    );
    }

    private double CalculateResidual(Equation<Matrix> equation, double[] alphas, Vector trueParametersValues)
    {
        Matrix.SumToDiagonal(equation.Matrix, alphas, _regularizedEquation.Matrix);

        Matrix.Multiply(_regularizedEquation.Matrix, _regularizedEquation.RightPart,
            _regularizedEquation.Solution);

        Vector.Subtract
        (
            equation.RightPart,
            Vector.Multiply
            (
                alphas,
                Vector.Subtract
                (
                    equation.Solution,
                    trueParametersValues,
                    _regularizedEquation.RightPart
                ),
                _regularizedEquation.RightPart
            ),
            _regularizedEquation.RightPart
        );

        return Vector.Subtract(
                _regularizedEquation.RightPart,
            _regularizedEquation.Solution, _regularizedEquation.RightPart).Norm;
    }

    private double[] FindPossibleAlphas(Equation<Matrix> equation, double[] alphas, Vector trueParametersValues)
    {
        for (; ; )
        {
            try
            {
                AssembleSLAE(equation, alphas, trueParametersValues);

                _gaussElimination.Solve(_regularizedEquation);

                break;
            }
            catch
            {
                for (var i = 0; i < alphas.Length; i++)
                {
                    alphas[i] *= 1.5;

                    Console.Write($"alpha{i} increased to {alphas[i]}                          \r");
                }
            }
        }

        return alphas;
    }

    private double[] FindBestAlphas(Equation<Matrix> equation, double[] alphas, Vector trueParametersValues)
    {
        bool stop;

        do
        {
            _regularizedEquation.Solution.Copy(_previousDeltas);

            AssembleSLAE(equation, alphas, trueParametersValues);

            _gaussElimination.Solve(_regularizedEquation);

            alphas = ChangeAlphas(equation, alphas, out stop);

        } while (!stop);

        return alphas;
    }

    private double[] ChangeAlphas(Equation<Matrix> equation, double[] alphas, out bool stop)
    {
        stop = true;

        Vector.Sum(equation.Solution, _regularizedEquation.Solution,
            _regularizedEquation.RightPart);

        for (var i = 0; i < alphas.Length; i++)
        {
            var changeRatio = _regularizedEquation.Solution[i] / _previousDeltas[i];

            if (CheckLocalConstraints(changeRatio) &&
                CheckGlobalConstraints(_regularizedEquation.RightPart[i])) continue;

            Console.Write("constraints not passed                          \r");

            alphas[i] *= 1.5;

            Console.Write($"alpha{i} increased to {alphas[i]}                          \r");

            stop = false;
        }

        return alphas;
    }

    private bool CheckLocalConstraints(double changeRatio)
    {
        return !(double.Max(1 / changeRatio, changeRatio) > 2d);
    }

    private bool CheckGlobalConstraints(double parameterValue)
    {
        return parameterValue is >= 1e-3 and <= 5d;
    }
}