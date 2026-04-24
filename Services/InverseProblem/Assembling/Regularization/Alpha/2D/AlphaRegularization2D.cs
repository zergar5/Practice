using Application.EquationSystems.Solvers;
using Application.Extensions;
using Application.InverseProblem._2D;
using Application.InverseProblem.Parameters;
using Application.MathObjects.Equation;
using Application.MathObjects.Matrices;
using Application.MathObjects.Vectors;
using Common.Extensions;
using DirectProblem.Core.Base;
using Domain;

namespace Application.InverseProblem.Assembling.Regularization.Alpha._2D;

public class AlphaRegularization2D : IAlphaRegularization
{
    private readonly IInverseProblemContextProvider<InverseProblem2DContext> _problemContextProvider;
    private readonly ISLAESolver<IMatrix<double>> _gaussElimination;

    private readonly Parameter[] _parameters;
    private readonly IVector<double> _alphas;
    private readonly IEquation<IMatrix<double>, double> _regularizedEquation;
    private readonly IVector<double> _currentDeltas;
    private readonly Interval _sigmaGlobalConstraintsInterval;
    private readonly double _sigmaLocalConstraintRatio;
    private readonly double _boundLocalConstraintDeviation;
    private readonly double _boundGlobalConstraintDeviation;
    private readonly double _alphaChangeRatio;

    public AlphaRegularization2D
    (
        IInverseProblemContextProvider<InverseProblem2DContext> problemContextProvider,
        ISLAESolver<IMatrix<double>> gaussElimination,
        Parameter[] parameters,
        Interval sigmaGlobalConstraintsInterval,
        double sigmaLocalConstraintRatio,
        double boundLocalConstraintDeviation,
        double boundGlobalConstraintDeviation,
        double alphaChangeRatio = 1.5
    )
    {
        _problemContextProvider = problemContextProvider;
        _gaussElimination = gaussElimination;
        _parameters = parameters;
        _sigmaGlobalConstraintsInterval = sigmaGlobalConstraintsInterval;
        _sigmaLocalConstraintRatio = sigmaLocalConstraintRatio;
        _boundLocalConstraintDeviation = boundLocalConstraintDeviation;
        _boundGlobalConstraintDeviation = boundGlobalConstraintDeviation;

        _alphas = new Vector<double>(parameters.Length);

        _regularizedEquation = new Equation<IMatrix<double>, double>(
            new Matrix<double>(parameters.Length),
            new Vector<double>(parameters.Length),
            new Vector<double>(parameters.Length)
        );

        _currentDeltas = new Vector<double>(parameters.Length);

        _alphaChangeRatio = alphaChangeRatio;
    }

    public IVector<double> Regularize(IEquation<IMatrix<double>, double> equation)
    {
        //SetupAlphas(equation.Matrix);

        //for (var i = 0; i < equation.Matrix.RowCount; i++)
        //{
        //    Console.WriteLine($"Initial alpha {i} {_alphas[i]:E16}");
        //}

        //FindInitialAlphas(equation);

        //for (var i = 0; i < _regularizedEquation.Solution.Count; i++)
        //{
        //    Console.WriteLine($"Initial deltas {i} {_regularizedEquation.Solution[i]:F6}");
        //}

        //FindBestAlphaForEachParameter(equation);

        //for (var i = 0; i < equation.Matrix.RowCount; i++)
        //{
        //    Console.WriteLine($"Final alpha {i} {_alphas[i]:E16}");
        //}

        //return _currentDeltas;

        AssembleRegularizedEquation(equation);

        _gaussElimination.Solve(_regularizedEquation);

        return _regularizedEquation.Solution;
    }

    private void SetupAlphas(IMatrix<double> matrix)
    {
        for (var i = 0; i < matrix.RowCount; i++)
        {
            _alphas[i] = matrix[i, i] * 1e-8;
        }
    }

    private void FindInitialAlphas(IEquation<IMatrix<double>, double> equation)
    {
        while (true)
        {
            try
            {
                AssembleRegularizedEquation(equation);

                _gaussElimination.Solve(_regularizedEquation);

                break;
            }
            catch
            {
                _alphas.Multiply(_alphaChangeRatio, _alphas);
            }
        }
    }

    private void FindBestAlphaForEachParameter(IEquation<IMatrix<double>, double> equation)
    {
        ChangeAlphasWithConstraints(equation, out var stop);

        do
        {
            AssembleRegularizedEquation(equation);

            _gaussElimination.Solve(_regularizedEquation);
            _regularizedEquation.Solution.Copy(_currentDeltas);

            ChangeAlphasWithConstraints(equation, out stop);

        } while (!stop);
    }

    private void AssembleRegularizedEquation(IEquation<IMatrix<double>, double> equation)
    {
        equation.Matrix.Copy(_regularizedEquation.Matrix);
        _regularizedEquation.Matrix.SumToDiagonal(_alphas, _regularizedEquation.Matrix);
        equation.RightPart.Copy(_regularizedEquation.RightPart);
    }

    private void ChangeAlphasWithConstraints(IEquation<IMatrix<double>, double> equation, out bool allAlphasIsBest)
    {
        allAlphasIsBest = true;

        var nextRegularizedSolution = equation.Solution.Sum(_regularizedEquation.Solution, _regularizedEquation.Solution);

        for (var i = 0; i < _alphas.Count; i++)
        {
            var parameterType = _parameters[i].Type;
            var parameterValue = nextRegularizedSolution[i];
            var parameterPreviousValue = equation.Solution[i];

            if (parameterType == ParameterType.Sigma)
            {
                var changeRatio = parameterPreviousValue / parameterValue;

                if (CheckSigmaParameterConstraints(parameterValue, changeRatio)) continue;
            }
            else if (parameterType is ParameterType.VerticalBound or ParameterType.HorizontalBound)
            {
                if (CheckBoundParameterConstraints(_parameters[i], parameterValue, parameterPreviousValue)) continue;
            }

            _alphas[i] *= _alphaChangeRatio;

            allAlphasIsBest = false;
        }
    }

    private bool CheckSigmaParameterConstraints(double parameterValue, double changeRatio)
    {
        var isLocalConstrainsPassed = !(double.Max(1 / changeRatio, changeRatio) > _sigmaLocalConstraintRatio);
        var isGlobalConstrainsPassed = parameterValue >= _sigmaGlobalConstraintsInterval.Begin &&
                                       parameterValue <= _sigmaGlobalConstraintsInterval.End;

        return isLocalConstrainsPassed && isGlobalConstrainsPassed;
    }

    private bool CheckBoundParameterConstraints(Parameter parameter, double parameterValue, double parameterPreviousValue)
    {
        var isLocalConstrainsPassed = parameterPreviousValue.Difference(parameterValue).LessOrEqualThan(_boundLocalConstraintDeviation);
        var isGlobalConstrainsPassed = CheckBoundParameterGlobalConstraints(parameter, parameterValue);

        return isLocalConstrainsPassed && isGlobalConstrainsPassed;
    }

    private bool CheckBoundParameterGlobalConstraints(Parameter parameter, double parameterValue)
    {
        var parameterType = parameter.Type;

        var previousParameterIndex = parameter.Index - 1;
        var previousParameterValue = GetBoundParameterValue(previousParameterIndex, parameterType);

        var nextParameterIndex = parameter.Index + 1;
        var nextParameterValue = GetBoundParameterValue(nextParameterIndex, parameterType);

        return parameterValue >= previousParameterValue && parameterValue <= nextParameterValue &&
               parameterValue.Difference(previousParameterValue).GreaterOrEqualThan(_boundGlobalConstraintDeviation) &&
               nextParameterValue.Difference(parameterValue).GreaterOrEqualThan(_boundGlobalConstraintDeviation);
    }

    private double GetBoundParameterValue(int index, ParameterType parameterType)
    {
        var problemContext = _problemContextProvider.Get();

        var parameterOrder = _parameters.FindIndex(p => p.Index == index && p.Type == parameterType);

        if (parameterOrder != -1)
        {
            return _regularizedEquation.Solution[parameterOrder];
        }

        return parameterType == ParameterType.VerticalBound
            ? problemContext.GridParameters.XControlPoints[index]
            : problemContext.GridParameters.YControlPoints[index];
    }
}