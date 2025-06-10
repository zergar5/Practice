using Application.MathObjects.Vectors;
using System.Numerics;

namespace Application.MathObjects.Equation;

public interface IEquation<out TMatrix, T> where T : INumberBase<T>
{
    public TMatrix Matrix { get; }
    public IVector<T> Solution { get; }
    public IVector<T> RightPart { get; }
}

public class Equation<TMatrix, T> : IEquation<TMatrix, T> where T : INumberBase<T>
{
    public TMatrix Matrix { get; }
    public IVector<T> Solution { get; }
    public IVector<T> RightPart { get; }

    public Equation(TMatrix matrix, IVector<T> solution, IVector<T> rightPart)
    {
        Matrix = matrix;
        Solution = solution;
        RightPart = rightPart;
    }
}