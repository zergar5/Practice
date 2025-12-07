using Application.MathObjects.Equation;
using Application.MathObjects.Vectors;

namespace Application.EquationSystems.Solvers;

public interface ISLAESolver<in TMatrix>
{
    public IVector<double> Solve(IEquation<TMatrix, double> equation);
}