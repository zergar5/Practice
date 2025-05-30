using System.Diagnostics.CodeAnalysis;
using System.Globalization;
using System.Numerics;

namespace Services.Matrices;

public abstract class MatrixBase<T> where T : INumber<T>
{
    public abstract int RowCount { get; }
    public abstract int ColumnCount { get; }
    public abstract T this[int i, int j] { get; set; }
}

public class Matrix<T> : MatrixBase<T> where T : INumber<T>
{
    private readonly T[,] _matrix;
    public override int RowCount => _matrix.GetLength(0);
    public override int ColumnCount => _matrix.GetLength(1);

    public Matrix(T[,] matrix)
    {
        _matrix = matrix;
    }

    public Matrix(int n) : this(new T[n, n]) { }
    public Matrix(int n, int m) : this(new T[n, m]) { }

    public override T this[int i, int j]
    {
        get => _matrix[i, j];
        set => _matrix[j, i] = value;
    }
}