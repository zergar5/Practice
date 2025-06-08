using Application.MathObjects.Matrices;
using System.Numerics;

namespace Application.FEM.Core.Assembling.Local;

public interface ILocalMatrix<T> where T : INumberBase<T>
{
    public int RowCount { get; }
    public int ColumnCount { get; }
    public KeyValuePair<(int, int), T> this[int i, int j] { get; }
}

public class LocalMatrix<T> : ILocalMatrix<T> where T : INumberBase<T>
{
    private readonly IMatrix<T> _matrix;
    private readonly int[] _indexesFromGlobal;

    public int RowCount => _matrix.RowCount;
    public int ColumnCount => _matrix.ColumnCount;

    // TODO: Подумать как можно сделать более удобные получаемые данные, например "выдирать" строку из матрицы в виде ReadOnlySpan
    public KeyValuePair<(int, int), T> this[int i, int j] => new((_indexesFromGlobal[i], _indexesFromGlobal[j]), _matrix[i, j]);

    public LocalMatrix(IMatrix<T> matrix, int[] indexesFromGlobal)
    {
        _matrix = matrix;
        _indexesFromGlobal = indexesFromGlobal;
    }
}