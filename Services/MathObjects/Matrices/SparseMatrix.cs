using Application.MathObjects.Vectors;
using System.Numerics;
using Common.Extensions;

namespace Application.MathObjects.Matrices;

public interface ISparseMatrix<T> where T : INumberBase<T>
{
    public int RowCount { get; }
    public int ColumnCount { get; }
    public IReadOnlyList<int> RowIndexes { get; }
    public IReadOnlyList<int> ColumnIndexes { get; }
    public T this[int i, int j] { get; set; }
    public ReadOnlySpan<int> this[int i] { get; }
    public ISparseMatrix<T> Copy(ISparseMatrix<T> sparseMatrix);
    public ISparseMatrix<T> Clone();
}

public abstract class SparseMatrixBase<T> : ISparseMatrix<T> where T : INumberBase<T>
{
    protected readonly int[] RowIndexesInner;
    protected readonly int[] ColumnIndexesInner;

    protected SparseMatrixBase(int[] rowIndexes, int[] columnIndexes)
    {
        RowIndexesInner = rowIndexes;
        ColumnIndexesInner = columnIndexes;
    }

    public abstract IReadOnlyList<int> RowIndexes { get; }
    public abstract IReadOnlyList<int> ColumnIndexes { get; }

    public abstract int RowCount { get; }
    public abstract int ColumnCount { get; }

    public ReadOnlySpan<int> this[int rowIndex] => ColumnIndexesInner[RowIndexesInner[rowIndex]..RowIndexesInner[rowIndex + 1]];

    public abstract T this[int rowIndex, int columnIndex] { get; set; }

    public abstract ISparseMatrix<T> Copy(ISparseMatrix<T> sparseMatrix);
    public abstract ISparseMatrix<T> Clone();

    protected int FindMatrixElementIndexInFlatArray(int rowIndex, int columnIndex) => Array.IndexOf(ColumnIndexesInner, columnIndex, RowIndexesInner[rowIndex],
        RowIndexesInner[rowIndex + 1] - RowIndexesInner[rowIndex]);
}

public class SparseMatrix<T> : SparseMatrixBase<T>, ISparseMatrix<T> where T : INumberBase<T>
{
    private readonly T[] _diagonal;
    private readonly T[] _lowerValues;
    private readonly T[] _upperValues;

    public override IReadOnlyList<int> RowIndexes => RowIndexesInner.AsReadOnly();
    public override IReadOnlyList<int> ColumnIndexes => ColumnIndexesInner.AsReadOnly();

    public override int RowCount => _diagonal.Length;
    public override int ColumnCount => _diagonal.Length;

    public override T this[int rowIndex, int columnIndex]
    {
        get
        {
            if (rowIndex < 0 || columnIndex < 0) throw new IndexOutOfRangeException(nameof(rowIndex) + nameof(columnIndex));

            if (rowIndex == columnIndex)
            {
                return _diagonal[rowIndex];
            }

            if (columnIndex > rowIndex)
            {
                (rowIndex, columnIndex) = (columnIndex, rowIndex);
                var index = FindMatrixElementIndexInFlatArray(rowIndex, columnIndex);
                return index != -1 ? _upperValues[index] : throw new ArgumentOutOfRangeException(nameof(rowIndex) + nameof(columnIndex));

            }
            else
            {
                var index = FindMatrixElementIndexInFlatArray(rowIndex, columnIndex);
                return index != -1 ? _lowerValues[index] : throw new ArgumentOutOfRangeException(nameof(rowIndex) + nameof(columnIndex));
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
                var index = FindMatrixElementIndexInFlatArray(rowIndex, columnIndex);
                if (index != -1) _upperValues[index] = value;
            }
            else
            {
                var index = FindMatrixElementIndexInFlatArray(rowIndex, columnIndex);
                if (index != -1) _lowerValues[index] = value;
            }
        }
    }

    public SparseMatrix(int[] rowIndexes, int[] columnIndexes) : base(rowIndexes, columnIndexes)
    {
        _diagonal = new T[rowIndexes.Length - 1];
        _lowerValues = new T[rowIndexes[^1]];
        _upperValues = new T[rowIndexes[^1]];
    }

    public SparseMatrix
    (
        int[] rowIndexes,
        int[] columnIndexes,
        T[] diagonal,
        T[] lowerValues,
        T[] upperValues
    ) : base(rowIndexes, columnIndexes)
    {
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
            RowIndexesInner.ToArray(),
            ColumnIndexesInner.ToArray(),
            _diagonal.ToArray(),
            _lowerValues.ToArray(),
            _upperValues.ToArray()
        );
    }
}

public static class SparseMatrixExtensions
{
    public static IVector<TResult> Multiply<TSelf, TOther, TResult>(this ISparseMatrix<TSelf> matrix, IVector<TOther> vector, IVector<TResult>? result = null)
        where TSelf : INumberBase<TSelf>
        where TOther : INumberBase<TOther>
        where TResult : INumberBase<TResult>
    {
        if (matrix.ColumnCount != vector.Count)
            throw new ArgumentOutOfRangeException($"{nameof(matrix)} and {nameof(vector)} must have same size");

        if (result == null) result = new Vectors.Vector<TResult>(matrix.RowCount);
        else result.Clear();

        for (var i = 0; i < matrix.RowCount; i++)
        {
            result[i] += TResult.CreateChecked(matrix[i, i]) * TResult.CreateChecked(vector[i]);

            foreach (var j in matrix[i])
            {
                result[i] += TResult.CreateChecked(matrix[i, j]) * TResult.CreateChecked(vector[j]);
                result[j] += TResult.CreateChecked(matrix[j, i]) * TResult.CreateChecked(vector[i]);
            }
        }

        return result;
    }

    public static IProfileMatrix<double> ToProfileMatrix(this ISparseMatrix<double> sparseMatrix)
    {
        var diagonal = new double[sparseMatrix.RowCount];
        var rowsIndexes = sparseMatrix.RowIndexes.ToArray();
        var lowerValues = new List<double>();
        var upperValues = new List<double>();

        for (var i = 1; i < rowsIndexes.Length; i++)
        {
            var previousRowIndex = i - 1;
            var rowBegin = previousRowIndex;

            diagonal[previousRowIndex] = sparseMatrix[previousRowIndex, previousRowIndex];

            foreach (var j in sparseMatrix[previousRowIndex])
            {
                if (sparseMatrix[previousRowIndex, j].EqualTo(0) && sparseMatrix[j, previousRowIndex].EqualTo(0)) 
                    continue;

                rowBegin = j;
                break;
            }

            rowsIndexes[i] = rowsIndexes[previousRowIndex] + (previousRowIndex - rowBegin);

            for (var k = rowsIndexes[previousRowIndex]; k < rowsIndexes[i]; k++, rowBegin++)
            {
                try
                {
                    lowerValues.Add(sparseMatrix[previousRowIndex, rowBegin]);
                    upperValues.Add(sparseMatrix[rowBegin, previousRowIndex]);
                }
                catch (ArgumentOutOfRangeException)
                {
                    lowerValues.Add(0d);
                    upperValues.Add(0d);
                }
            }
        }

        return new ProfileMatrix<double>(rowsIndexes, diagonal, lowerValues, upperValues);
    }
}