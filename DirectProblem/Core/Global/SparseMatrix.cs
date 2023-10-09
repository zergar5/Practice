﻿using DirectProblem.Core.Base;

namespace DirectProblem.Core.Global;

public class SparseMatrix
{
    private readonly double[] _diagonal;
    private readonly double[] _lowerValues;
    private readonly double[] _upperValues;
    private readonly int[] _rowsIndexes;
    private readonly int[] _columnsIndexes;
    public ReadOnlySpan<int> RowsIndexes => new(_rowsIndexes);
    public ReadOnlySpan<int> ColumnsIndexes => new(_columnsIndexes);

    public int Count => _diagonal.Length;

    public ReadOnlySpan<int> this[int rowIndex] => ColumnsIndexes[RowsIndexes[rowIndex]..RowsIndexes[rowIndex + 1]];

    public double this[int rowIndex, int columnIndex]
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
                var index = this[rowIndex].IndexOf(columnIndex);
                return _upperValues[index];

            }
            else
            {
                var index = this[rowIndex].IndexOf(columnIndex);
                return _lowerValues[index];
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
                var index = this[rowIndex].IndexOf(columnIndex);
                _upperValues[index] = value;

            }
            else
            {
                var index = this[rowIndex].IndexOf(columnIndex);
                _lowerValues[index] = value;
            }
        }
    }

    public SparseMatrix(int[] rowsIndexes, int[] columnsIndexes)
    {
        _rowsIndexes = rowsIndexes;
        _columnsIndexes = columnsIndexes;
        _diagonal = new double[rowsIndexes.Length - 1];
        _lowerValues = new double[rowsIndexes[^1]];
        _upperValues = new double[rowsIndexes[^1]];
    }

    public SparseMatrix
    (
        int[] rowsIndexes,
        int[] columnsIndexes,
        double[] diagonal,
        double[] lowerValues,
        double[] upperValues
    )
    {
        _rowsIndexes = rowsIndexes;
        _columnsIndexes = columnsIndexes;
        _diagonal = diagonal;
        _lowerValues = lowerValues;
        _upperValues = upperValues;
    }

    public static Vector Multiply(SparseMatrix matrix, Vector vector, Vector? result = null)
    {
        if (matrix.Count != vector.Count)
            throw new ArgumentOutOfRangeException(
                $"{nameof(matrix)} and {nameof(vector)} must have same size");

        result ??= new Vector(matrix.Count);

        var rowsIndexes = matrix.RowsIndexes;

        for (var i = 0; i < matrix.Count; i++)
        {
            result[i] += matrix[i, i] * vector[i];

            for (var j = rowsIndexes[i]; j < rowsIndexes[i + 1]; j++)
            {
                result[i] += matrix[i, j] * vector[j];
                result[j] += matrix[i, j] * vector[i];
            }
        }

        return result;
    }

    public SparseMatrix Clone()
    {
        var rowIndexes = new int[_rowsIndexes.Length];
        var columnIndexes = new int[_columnsIndexes.Length];
        var diagonal = new double[_diagonal.Length];
        var lowerValues = new double[_lowerValues.Length];
        var upperValues = new double[_upperValues.Length];

        Array.Copy(_rowsIndexes, rowIndexes, _rowsIndexes.Length);
        Array.Copy(_columnsIndexes, columnIndexes, _columnsIndexes.Length);
        Array.Copy(_diagonal, diagonal, _diagonal.Length);
        Array.Copy(_lowerValues, lowerValues, _lowerValues.Length);
        Array.Copy(_upperValues, upperValues, _upperValues.Length);

        return new SparseMatrix(rowIndexes, columnIndexes, diagonal, lowerValues, upperValues);
    }

    public SparseMatrix Copy(SparseMatrix sparseMatrix)
    {
        Array.Copy(_diagonal, sparseMatrix._diagonal, _diagonal.Length);
        Array.Copy(_lowerValues, sparseMatrix._lowerValues, _lowerValues.Length);
        Array.Copy(_upperValues, sparseMatrix._upperValues, _upperValues.Length);

        return sparseMatrix;
    }
}