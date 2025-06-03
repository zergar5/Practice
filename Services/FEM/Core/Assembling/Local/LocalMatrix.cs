using System.Numerics;
using Services.MathObjects.Matrices;

namespace Application.FEM.Core.Assembling.Local;

public interface ILocalMatrix<T> where T : INumber<T>
{
    public int RowCount { get; }
    public int ColumnCount { get; }
    public KeyValuePair<(int, int), T> this[int i, int j] { get; }
}

public class LocalMatrix<T> : ILocalMatrix<T> where T : INumber<T>
{
    private readonly IMatrix<T> _matrix;
    private readonly int[] _indexesFromGlobal;

    public int RowCount => _matrix.RowCount;
    public int ColumnCount => _matrix.ColumnCount;

    public KeyValuePair<(int, int), T> this[int i, int j] => new((_indexesFromGlobal[i], _indexesFromGlobal[j]), _matrix[i, j]);

    public LocalMatrix(IMatrix<T> matrix, int[] indexesFromGlobal)
    {
        _matrix = matrix;
        _indexesFromGlobal = indexesFromGlobal;
    }
}