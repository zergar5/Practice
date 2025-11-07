using System.Collections.Concurrent;
using System.Numerics;
using Application.MathObjects.Matrices;

namespace Application.MathObjects.Vectors;

public static class VectorPool<T> where T : INumberBase<T>
{
    private static readonly ConcurrentDictionary<int, ConcurrentQueue<IVector<T>>> VectorPools = new();

    public static IVector<T> Rent(int size)
    {
        if (VectorPools.TryGetValue(size, out var pool))
        {
            if (pool.TryDequeue(out var vector))
            {
                return vector;
            }
        }
        else
        {
            VectorPools[size] = new ConcurrentQueue<IVector<T>>();
        }

        return new Vector<T>(size);
    }

    public static void Return(IVector<T> vector)
    {
        if (vector.Count == 0)
        {
            return;
        }

        VectorPools.TryGetValue(vector.Count, out var pool);

        if (pool == null) return;

        pool.Enqueue(vector);
        vector.Clear();
    }
}