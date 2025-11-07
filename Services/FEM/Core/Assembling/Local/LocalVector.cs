using Application.MathObjects.Vectors;
using System.Numerics;

namespace Application.FEM.Core.Assembling.Local;

public interface ILocalVector<out T> where T : INumberBase<T>
{
    public int Count { get; }
    public T this[int i] { get; }
    public int GetGlobalIndexOfLocal(int index);
}

public class LocalVector<T> : ILocalVector<T> where T : INumberBase<T>
{
    private readonly IVector<T> _vector;
    private readonly int[] _indexesFromGlobal;

    public int Count => _vector.Count;
    public T this[int i] => _vector[i];

    public LocalVector(IVector<T> vector, int[] indexesFromGlobal)
    {
        _vector = vector;
        _indexesFromGlobal = indexesFromGlobal;
    }

    public int GetGlobalIndexOfLocal(int index) => _indexesFromGlobal[index];
}