using DirectProblem.Core.Base;
using DirectProblem.Core.Global;
using InverseProblem.Parameters;

namespace InverseProblem.SLAE;

public class Regularizer
{
    private readonly GaussElimination _gaussElimination;
    private readonly double[] _alphas;
    private readonly Equation<Matrix> _regularizedEquation;
    private readonly Vector _previousSolution;

    public Regularizer(GaussElimination gaussElimination, Parameter[] parameters)
    {
        _gaussElimination = gaussElimination;
        _alphas = new double[parameters.Length];
        _regularizedEquation = new Equation<Matrix>(
            new Matrix(parameters.Length),
            new Vector(parameters.Length),
            new Vector(parameters.Length)
            );
        _previousSolution = new Vector(parameters.Length);
    }

    public Equation<Matrix> Regularize(Equation<Matrix> equation, out double[] alphas)
    {
        alphas = SetupAlphas(equation.Matrix);

        alphas = FindPossibleAlphas(equation, alphas);

        alphas = FindBestAlphas(equation, alphas);

        AssembleSLAE(equation, alphas);

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

    private void AssembleSLAE(Equation<Matrix> equation, double[] alphas)
    {
        equation.Matrix.Copy(_regularizedEquation.Matrix);
        Matrix.SumToDiagonal(equation.Matrix, alphas, _regularizedEquation.Matrix);

        equation.RightPart.Copy(_regularizedEquation.RightPart);
    }

    private double[] FindPossibleAlphas(Equation<Matrix> equation, double[] alphas)
    {
        for (; ; )
        {
            try
            {
                AssembleSLAE(equation, alphas);

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

    private double[] FindBestAlphas(Equation<Matrix> equation, double[] alphas)
    {
        bool stop;

        do
        {
            AssembleSLAE(equation, alphas);

            _gaussElimination.Solve(_regularizedEquation);

            alphas = ChangeAlphas(equation, alphas, out stop);

        } while (!stop);

        return alphas;
    }

    private double[] ChangeAlphas(Equation<Matrix> equation, double[] alphas, out bool stop)
    {
        stop = true;

        Vector.Sum(equation.Solution, _regularizedEquation.Solution,
            _regularizedEquation.Solution);

        for (var i = 0; i < alphas.Length; i++)
        {
            var changeRatio = equation.Solution[i] / _regularizedEquation.Solution[i];

            if (CheckLocalConstraints(changeRatio) &&
                CheckGlobalConstraints(_regularizedEquation.Solution[i])) continue;

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