using System.Collections;
using System.Numerics;

namespace Services.MathObjects.Vectors;

public interface IVector<T> : IEnumerable<T> where T : INumber<T>
{
    public int Count { get; }
    public T this[int i] { get; set; }
    public double Norm { get; }
    public double ScalarProduct(IVector<T> vectorA, IVector<T> vectorB);
    public double ScalarProduct(IVector<T> vector);
    public IVector<T> Sum(IVector<T> vectorA, IVector<T> vectorB, IVector<T>? result = null);
    public IVector<T> Subtract(IVector<T> vectorA, IVector<T> vectorB, IVector<T>? result = null);
    public IVector<T> Multiply(T number, IVector<T> vector, IVector<T>? result = null);
    public void Clear();
    public IVector<T> Clone();
    public IVector<T> Copy(IVector<T> vector);
}

public abstract class VectorBase<T> : IVector<T> where T : INumber<T>
{
    public abstract int Count { get; }
    public abstract T this[int i] { get; set; }
    public abstract double Norm { get; }

    public virtual double ScalarProduct(IVector<T> vectorA, IVector<T> vectorB) => vectorA.Select((t, i) => double.CreateChecked(t * vectorB[i])).Sum();
    public virtual double ScalarProduct(IVector<T> vector) => ScalarProduct(this, vector);
    public virtual IVector<T> Sum(IVector<T> vectorA, IVector<T> vectorB, IVector<T>? result = null)
    {
        if (vectorA.Count != vectorB.Count)
            throw new ArgumentOutOfRangeException($"{nameof(vectorA)} and {nameof(vectorB)} must have same size");

        result ??= new Vector<T>(vectorA.Count);

        for (var i = 0; i < vectorA.Count; i++)
        {
            result[i] = vectorA[i] + vectorB[i];
        }

        return result;
    }

    public virtual IVector<T> Subtract(IVector<T> vectorA, IVector<T> vectorB, IVector<T>? result = null)
    {
        if (vectorA.Count != vectorB.Count)
            throw new ArgumentOutOfRangeException(
                $"{nameof(vectorA)} and {nameof(vectorB)} must have same size");

        result ??= new Vector<T>(vectorA.Count);

        for (var i = 0; i < vectorA.Count; i++)
        {
            result[i] = vectorA[i] - vectorB[i];
        }

        return result;
    }

    public virtual IVector<T> Multiply(T number, IVector<T> vector, IVector<T>? result = null)
    {
        result ??= new Vector<T>(vector.Count);

        for (var i = 0; i < vector.Count; i++)
        {
            result[i] = number * vector[i];
        }

        return result;
    }

    public abstract void Clear();
    public abstract IVector<T> Clone();
    public virtual IVector<T> Copy(IVector<T> vector)
    {
        if (Count != vector.Count)
            throw new ArgumentOutOfRangeException($"Source and {nameof(vector)} must have same size");

        for (var i = 0; i < vector.Count; i++)
        {
            vector[i] = this[i];
        }

        return vector;
    }

    public abstract IEnumerator<T> GetEnumerator();
    IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();
}


public class Vector<T> : VectorBase<T> where T : INumber<T>
{
    private readonly T[] _vector;

    public override int Count => _vector.Length;
    public override double Norm => Math.Sqrt(ScalarProduct(this, this));

    public Vector(T[] vector)
    {
        _vector = vector;
    }
    public Vector(int n) : this(new T[n]) { }

    public override T this[int i]
    {
        get => _vector[i];
        set => _vector[i] = value;
    }

    public override void Clear()
    {
        Array.Clear(_vector);
    }

    public override IVector<T> Clone()
    {
        var clone = (T[])_vector.Clone();

        return new Vector<T>(clone);
    }

    public override IEnumerator<T> GetEnumerator() => ((IEnumerable<T>)_vector).GetEnumerator();
}