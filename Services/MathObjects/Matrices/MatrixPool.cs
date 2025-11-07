using System.Buffers;
using System.Collections.Concurrent;
using System.Numerics;

namespace Application.MathObjects.Matrices;

public static class MatrixPool<T> where T : INumberBase<T>
{
    private static readonly ConcurrentDictionary<(int rows, int columns), ConcurrentQueue<IMatrix<T>>> MatrixPools = new();

    public static IMatrix<T> Rent(int matrixSize) => Rent(matrixSize, matrixSize);

    public static IMatrix<T> Rent(int rows, int columns)
    {
        if (MatrixPools.TryGetValue((rows, columns), out var pool))
        {
            if (pool.TryDequeue(out var matrix))
            {
                return matrix;
            }
        }
        else
        {
            MatrixPools[(rows, columns)] = new ConcurrentQueue<IMatrix<T>>();
        }

        return new Matrix<T>(rows, columns);
    }

    public static void Return(IMatrix<T> matrix)
    {
        if (matrix.RowCount == 0 || matrix.ColumnCount == 0)
        {
            return;
        }

        MatrixPools.TryGetValue((matrix.RowCount, matrix.ColumnCount), out var pool);

        if (pool == null) return;

        pool.Enqueue(matrix);
        matrix.Clear();
    }
}