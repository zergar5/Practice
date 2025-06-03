using System.Numerics;
using Services.MathObjects.Matrices;
using Services.MathObjects.Vectors;

namespace Services.MathObjects.Equation;

public interface IEquation<out TMatrix, T>
    where TMatrix : IMatrix<T>, ISparseMatrix<T>
    where T : INumber<T>
{
    public TMatrix Matrix { get; }
    public IVector<T> Solution { get; }
    public IVector<T> RightPart { get; }
}

public class Equation<TMatrix, T> : IEquation<TMatrix, T>
    where TMatrix : IMatrix<T>, ISparseMatrix<T>
    where T : INumber<T>
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