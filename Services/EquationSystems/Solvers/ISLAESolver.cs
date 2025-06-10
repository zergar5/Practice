using Application.MathObjects.Equation;
using Application.MathObjects.Vectors;
using System.Numerics;

namespace Application.EquationSystems.Solvers;

public interface ISLAESolver<in TMatrix, T> where T : INumberBase<T>
{
    public IVector<T> Solve(IEquation<TMatrix, T> equation);
}