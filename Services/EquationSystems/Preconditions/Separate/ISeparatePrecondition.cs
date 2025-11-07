using Application.MathObjects.Vectors;
using System.Numerics;

namespace Application.EquationSystems.Preconditions.Separate;

public interface ISeparatePrecondition<in TMatrix, T> where T : INumberBase<T>
{
    public void DecomposeMatrix(TMatrix matrix);
    public IVector<T> ForwardElimination(IVector<T> vector, IVector<T>? result = null);
    public IVector<T> BackSubstitution(IVector<T> vector, IVector<T>? result = null);
}