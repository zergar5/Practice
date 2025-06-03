using System.Numerics;
using Services.MathObjects.Vectors;

namespace Services.MathObjects.Matrices;

public interface IMatrix<T> where T : INumber<T>
{
    public int RowCount { get; }
    public int ColumnCount { get; }
    public T this[int i, int j] { get; set; }
    public IMatrix<T> Sum(IMatrix<T> matrixA, IMatrix<T> matrixB, IMatrix<T>? result = null);
    public IMatrix<T> Multiply(T coefficient, IMatrix<T> matrix, IMatrix<T>? result = null);
    public IVector<T> Multiply(IMatrix<T> matrix, IVector<T> vector, IVector<T>? result = null);
    public IMatrix<T> Copy(IMatrix<T> matrix);
    public IMatrix<T> Clone();
}

public abstract class MatrixBase<T> : IMatrix<T> where T : INumber<T>
{
    public abstract int RowCount { get; }
    public abstract int ColumnCount { get; }
    public abstract T this[int i, int j] { get; set; }

    public virtual IMatrix<T> Sum(IMatrix<T> matrixA, IMatrix<T> matrixB, IMatrix<T>? result = null)
    {
        if (matrixA.RowCount != matrixB.RowCount || matrixA.ColumnCount != matrixB.ColumnCount)
            throw new ArgumentOutOfRangeException($"{nameof(matrixA)} and {nameof(matrixB)} must have same size");

        result ??= new Matrix<T>(matrixA.RowCount);

        for (var i = 0; i < matrixA.RowCount; i++)
        {
            for (var j = 0; j < matrixB.ColumnCount; j++)
            {
                result[i, j] = matrixA[i, j] + matrixB[i, j];
            }
        }

        return result;
    }

    public virtual IMatrix<T> Multiply(T coefficient, IMatrix<T> matrix, IMatrix<T>? result = null)
    {
        result ??= new Matrix<T>(matrix.RowCount);

        for (var i = 0; i < matrix.RowCount; i++)
        {
            for (var j = 0; j < matrix.ColumnCount; j++)
            {
                result[i, j] = coefficient * matrix[i, j];
            }
        }

        return result;
    }

    public virtual IVector<T> Multiply(IMatrix<T> matrix, IVector<T> vector, IVector<T>? result = null)
    {
        if (matrix.RowCount != vector.Count)
            throw new ArgumentOutOfRangeException($"{nameof(matrix)} and {nameof(vector)} must have same size");

        if (result == null) result = new Vectors.Vector<T>(matrix.RowCount);
        else result.Clear();

        for (var i = 0; i < matrix.RowCount; i++)
        {
            for (var j = 0; j < matrix.ColumnCount; j++)
            {
                result[i] += matrix[i, j] * vector[j];
            }
        }

        return result;
    }

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

    public virtual IMatrix<T> Clone()
    {
        throw new NotImplementedException();
    }
}

public class Matrix<T> : MatrixBase<T> where T : INumber<T>
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
        set => Values[j, i] = value;
    }
}