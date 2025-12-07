using Application.MathObjects.Equation;
using Application.MathObjects.Matrices;
using Application.MathObjects.Vectors;

namespace Application.InverseProblem.Assembling.Regularization.Alpha;

public interface IAlphaRegularization
{
    public IVector<double> Regularize(IEquation<IMatrix<double>, double> equation);
}