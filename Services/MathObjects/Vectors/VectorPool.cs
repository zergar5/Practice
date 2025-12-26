using System.Collections.Concurrent;
using System.Numerics;

namespace Application.MathObjects.Vectors;

public static class VectorPool<T> where T : INumberBase<T>
{
    private static readonly ConcurrentDictionary<int, ConcurrentQueue<IVector<T>>> VectorPools = new();

    public static IVector<T> Rent(int size)
    {
        var pool = VectorPools.GetOrAdd(size, _ => new ConcurrentQueue<IVector<T>>());

        return pool.TryDequeue(out var vector) ? vector : new Vector<T>(size);
    }

    public static void Return(IVector<T> vector)
    {
        if (vector.Count == 0)
        {
            return;
        }

        VectorPools.TryGetValue(vector.Count, out var pool);

        if (pool == null) return;

        vector.Clear();
        pool.Enqueue(vector);
    }
}