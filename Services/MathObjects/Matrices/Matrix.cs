using Application.MathObjects.Vectors;
using DirectProblem.Core.Base;
using System.Numerics;

namespace Application.MathObjects.Matrices;

public interface IMatrix<T> where T : INumberBase<T>
{
    public int RowCount { get; }
    public int ColumnCount { get; }
    public T this[int i, int j] { get; set; }
    public IMatrix<T> Copy(IMatrix<T> matrix);
    public IMatrix<T> Clone();
    public void Clear();
}

public abstract class MatrixBase<T> : IMatrix<T> where T : INumberBase<T>
{
    public abstract int RowCount { get; }
    public abstract int ColumnCount { get; }
    public abstract T this[int i, int j] { get; set; }

    public virtual IMatrix<T> Copy(IMatrix<T> matrix)
    {
        if (RowCount != matrix.RowCount || ColumnCount != matrix.ColumnCount)
            throw new ArgumentOutOfRangeException($"Source and {nameof(matrix)} must have same size");

        for (var i = 0; i < matrix.RowCount; i++)
        {
            for (var j = 0; j < matrix.ColumnCount; j++)
            {
                matrix[i, j] = this[i, j];
            }
        }

        return matrix;
    }

    public virtual IMatrix<T> Clone() => Copy(new Matrix<T>(RowCount, ColumnCount));

    public void Clear()
    {
        for (var i = 0; i < RowCount; i++)
        {
            for (var j = 0; j < ColumnCount; j++)
            {
                this[i, j] = T.Zero;
            }
        }
    }
}

public class Matrix<T> : MatrixBase<T> where T : INumberBase<T>
{
    protected readonly T[,] Values;
    public override int RowCount => Values.GetLength(0);
    public override int ColumnCount => Values.GetLength(1);

    public Matrix(T[,] values)
    {
        Values = values;
    }

    public Matrix(int n) : this(new T[n, n]) { }
    public Matrix(int n, int m) : this(new T[n, m]) { }

    public override T this[int i, int j]
    {
        get => Values[i, j];
        set => Values[i, j] = value;
    }
}

public static class MatrixExtensions
{
    public static IMatrix<TResult> Sum<TSelf, TOther, TResult>(this IMatrix<TSelf> matrixA, IMatrix<TOther> matrixB, IMatrix<TResult>? result = null)
        where TSelf : INumberBase<TSelf>
        where TOther : INumberBase<TOther>
        where TResult : INumberBase<TResult>
    {
        if (matrixA.RowCount != matrixB.RowCount || matrixA.ColumnCount != matrixB.ColumnCount)
            throw new ArgumentOutOfRangeException($"{nameof(matrixA)} and {nameof(matrixB)} must have same size");

        result ??= new Matrix<TResult>(matrixA.RowCount);

        for (var i = 0; i < matrixA.RowCount; i++)
        {
            for (var j = 0; j < matrixA.ColumnCount; j++)
            {
                result[i, j] = TResult.CreateChecked(matrixA[i, j]) + TResult.CreateChecked(matrixB[i, j]);
            }
        }

        return result;
    }

    public static IMatrix<TResult> Multiply<TSelf, TCoefficient, TResult>(this IMatrix<TSelf> matrix, TCoefficient coefficient, IMatrix<TResult>? result = null)
        where TSelf : INumberBase<TSelf>
        where TCoefficient : INumberBase<TCoefficient>
        where TResult : INumberBase<TResult>
    {
        result ??= new Matrix<TResult>(matrix.RowCount);

        for (var i = 0; i < matrix.RowCount; i++)
        {
            for (var j = 0; j < matrix.ColumnCount; j++)
            {
                result[i, j] = TResult.CreateChecked(matrix[i, j]) * TResult.CreateChecked(coefficient);
            }
        }

        return result;
    }

    public static IVector<TResult> Multiply<TSelf, TOther, TResult>(this IMatrix<TSelf> matrix, IVector<TOther> vector, IVector<TResult>? result = null)
        where TSelf : INumberBase<TSelf>
        where TOther : INumberBase<TOther>
        where TResult : INumberBase<TResult>
    {
        if (matrix.RowCount != vector.Count)
            throw new ArgumentOutOfRangeException($"{nameof(matrix)} and {nameof(vector)} must have same size");

        if (result == null) result = new Vectors.Vector<TResult>(matrix.RowCount);
        else result.Clear();

        for (var i = 0; i < matrix.RowCount; i++)
        {
            for (var j = 0; j < matrix.ColumnCount; j++)
            {
                result[i] += TResult.CreateChecked(matrix[i, j]) * TResult.CreateChecked(vector[j]);
            }
        }

        return result;
    }
}