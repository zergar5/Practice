using System.Collections.Concurrent;
using System.Numerics;

namespace Application.MathObjects.Matrices;

public static class MatrixPool<T> where T : INumberBase<T>
{
    private static readonly ConcurrentDictionary<(int rows, int columns), ConcurrentQueue<IMatrix<T>>> MatrixPools = new();

    public static IMatrix<T> Rent(int matrixSize) => Rent(matrixSize, matrixSize);

    public static IMatrix<T> Rent(int rows, int columns)
    {
        var pool = MatrixPools.GetOrAdd((rows, columns), _ => new ConcurrentQueue<IMatrix<T>>());

        return pool.TryDequeue(out var matrix) ? matrix : new Matrix<T>(rows, columns);
    }

    public static void Return(IMatrix<T> matrix)
    {
        if (matrix.RowCount == 0 || matrix.ColumnCount == 0)
        {
            return;
        }

        MatrixPools.TryGetValue((matrix.RowCount, matrix.ColumnCount), out var pool);

        if (pool == null) return;

        matrix.Clear();
        pool.Enqueue(matrix);
    }
}