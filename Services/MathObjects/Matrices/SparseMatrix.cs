using System.Collections.Immutable;
using System.Numerics;
using Services.MathObjects.Vectors;

namespace Services.MathObjects.Matrices;

public interface ISparseMatrix<T> where T : INumber<T>
{
    public int RowCount { get; }
    public int ColumnCount { get; }
    public ImmutableArray<int> RowIndexes { get; }
    public ImmutableArray<int> ColumnIndexes { get; }
    public T this[int i, int j] { get; set; }
    public ImmutableArray<int> this[int i] { get; }
    public IVector<T> Multiply(ISparseMatrix<T> matrix, IVector<T> vector, IVector<T>? result = null);
    public ISparseMatrix<T> Copy(ISparseMatrix<T> sparseMatrix);
    public ISparseMatrix<T> Clone();
}

public abstract class SparseMatrixBase<T> : ISparseMatrix<T> where T : INumber<T>
{
    public abstract ImmutableArray<int> RowIndexes { get; }
    public abstract ImmutableArray<int> ColumnIndexes { get; }

    public abstract int RowCount { get; }
    public abstract int ColumnCount { get; }

    public virtual ImmutableArray<int> this[int rowIndex] => ColumnIndexes[RowIndexes[rowIndex]..RowIndexes[rowIndex + 1]];
    public abstract T this[int rowIndex, int columnIndex] { get; set; }

    public virtual IVector<T> Multiply(ISparseMatrix<T> matrix, IVector<T> vector, IVector<T>? result = null)
    {
        if (matrix.ColumnCount != vector.Count)
            throw new ArgumentOutOfRangeException($"{nameof(matrix)} and {nameof(vector)} must have same size");

        if (result == null) result = new Vectors.Vector<T>(matrix.RowCount);
        else result.Clear();

        for (var i = 0; i < matrix.RowCount; i++)
        {
            result[i] += matrix[i, i] * vector[i];

            foreach (var j in matrix[i])
            {
                result[i] += matrix[i, j] * vector[j];
                result[j] += matrix[j, i] * vector[i];
            }
        }

        return result;
    }

    public abstract ISparseMatrix<T> Copy(ISparseMatrix<T> sparseMatrix);
    public abstract ISparseMatrix<T> Clone();
}

public class SparseMatrix<T> : SparseMatrixBase<T>, ISparseMatrix<T> where T : INumber<T>
{
    private readonly int[] _rowIndexes;
    private readonly int[] _columnIndexes;
    private readonly T[] _diagonal;
    private readonly T[] _lowerValues;
    private readonly T[] _upperValues;

    public override ImmutableArray<int> RowIndexes => [.. _rowIndexes];
    public override ImmutableArray<int> ColumnIndexes => [.. _columnIndexes];

    public override int RowCount => _diagonal.Length;
    public override int ColumnCount => _diagonal.Length;

    public override T this[int rowIndex, int columnIndex]
    {
        get
        {
            if (rowIndex < 0 || columnIndex < 0) throw new ArgumentOutOfRangeException(nameof(rowIndex));

            if (rowIndex == columnIndex)
            {
                return _diagonal[rowIndex];
            }

            if (columnIndex > rowIndex)
            {
                (rowIndex, columnIndex) = (columnIndex, rowIndex);
                var index = IndexOf(rowIndex, columnIndex);
                return index != -1 ? _upperValues[index] : default;

            }
            else
            {
                var index = IndexOf(rowIndex, columnIndex);
                return index != -1 ? _lowerValues[index] : default;
            }
        }
        set
        {
            if (rowIndex < 0 || columnIndex < 0) throw new ArgumentOutOfRangeException(nameof(rowIndex));

            if (rowIndex == columnIndex)
            {
                _diagonal[rowIndex] = value;
                return;
            }

            if (columnIndex > rowIndex)
            {
                (rowIndex, columnIndex) = (columnIndex, rowIndex);
                var index = IndexOf(rowIndex, columnIndex);
                if (index != -1) _upperValues[index] = value;
            }
            else
            {
                var index = IndexOf(rowIndex, columnIndex);
                if (index != -1) _lowerValues[index] = value;
            }
        }
    }

    public SparseMatrix(int[] rowIndexes, int[] columnIndexes)
    {
        _rowIndexes = rowIndexes;
        _columnIndexes = columnIndexes;
        _diagonal = new T[rowIndexes.Length - 1];
        _lowerValues = new T[rowIndexes[^1]];
        _upperValues = new T[rowIndexes[^1]];
    }

    private SparseMatrix
    (
        int[] rowIndexes,
        int[] columnIndexes,
        T[] diagonal,
        T[] lowerValues,
        T[] upperValues
    )
    {
        _rowIndexes = rowIndexes;
        _columnIndexes = columnIndexes;
        _diagonal = diagonal;
        _lowerValues = lowerValues;
        _upperValues = upperValues;
    }

    public override ISparseMatrix<T> Copy(ISparseMatrix<T> sparseMatrix)
    {
        throw new NotImplementedException();
    }

    public override ISparseMatrix<T> Clone()
    {
        return new SparseMatrix<T>
        (
            _rowIndexes.ToArray(),
            _columnIndexes.ToArray(),
            _diagonal.ToArray(),
            _lowerValues.ToArray(),
            _upperValues.ToArray()
        );
    }

    private int IndexOf(int rowIndex, int columnIndex) => ColumnIndexes.IndexOf(columnIndex, RowIndexes[rowIndex],
        RowIndexes[rowIndex + 1] - RowIndexes[rowIndex]);
}