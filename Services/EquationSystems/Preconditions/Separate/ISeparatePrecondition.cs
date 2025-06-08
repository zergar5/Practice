using System.Numerics;
using Application.MathObjects.Vectors;
using DirectProblem.Core.Global;

namespace Application.EquationSystems.Preconditions.Separate;

public interface ISeparatePrecondition<T> where T : INumberBase<T>
{
    public IVector<T> ForwardElimination(IVector<T> vector, IVector<T>? result = null);
    public IVector<T> BackSubstitution(IVector<T> vector, IVector<T>? result = null);
}