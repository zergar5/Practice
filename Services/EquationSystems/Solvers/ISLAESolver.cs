using System.Numerics;
using Application.MathObjects.Equation;
using Application.MathObjects.Vectors;
using DirectProblem.Core.Global;

namespace Application.EquationSystems.Solvers;

public interface ISLAESolver<in TMatrix, T> where T : INumberBase<T>
{
    public IVector<T> Solve(IEquation<TMatrix, T> equation);
}