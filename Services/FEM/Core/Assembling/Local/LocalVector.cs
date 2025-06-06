using Services.MathObjects.Vectors;
using System.Numerics;

namespace Application.FEM.Core.Assembling.Local;

public interface ILocalVector<T> where T : INumber<T>
{
    public int Count { get; }
    public KeyValuePair<int, T> this[int i] { get; }
}

public class LocalVector<T> : ILocalVector<T> where T : INumber<T>
{
    private readonly IVector<T> _vector;
    private readonly int[] _indexesFromGlobal;

    public int Count => _vector.Count;
    public KeyValuePair<int, T> this[int i] => new(_indexesFromGlobal[i], _vector[i]);

    public LocalVector(IVector<T> vector, int[] indexesFromGlobal)
    {
        _vector = vector;
        _indexesFromGlobal = indexesFromGlobal;
    }
}