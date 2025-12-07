using Application.MathObjects.Matrices;
using System.Buffers;
using System.Numerics;

namespace Application.FEM.Core.Assembling.Local;

public interface ILocalMatrix<out T> : IDisposable where T : INumberBase<T>
{
    public int RowCount { get; }
    public int ColumnCount { get; }
    public T this[int i, int j] { get; }
    public int GetGlobalIndexOfLocal(int index);
}

public class LocalMatrix<T> : ILocalMatrix<T> where T : INumberBase<T>
{
    private readonly IMatrix<T> _matrix;
    private readonly int[] _indexesFromGlobal;

    public int RowCount => _matrix.RowCount;
    public int ColumnCount => _matrix.ColumnCount;

    public T this[int i, int j] => _matrix[i, j];

    public LocalMatrix(IMatrix<T> matrix, int[] indexesFromGlobal)
    {
        _matrix = matrix;
        _indexesFromGlobal = indexesFromGlobal;
    }

    public int GetGlobalIndexOfLocal(int index) => _indexesFromGlobal[index];

    public void Dispose()
    {
        MatrixPool<T>.Return(_matrix);
        ArrayPool<int>.Shared.Return(_indexesFromGlobal);
    }
}