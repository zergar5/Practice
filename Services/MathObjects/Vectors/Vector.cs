using System.Collections;
using System.Numerics;

namespace Application.MathObjects.Vectors;

public interface IVector<T> : IEnumerable<T> where T : INumberBase<T>
{
    public int Count { get; }
    public T this[int i] { get; set; }
    public double Norm { get; }
    public double ScalarProduct(IVector<T> vector);
    public double ScalarProduct();
    public void Clear();
    public IVector<T> Clone();
    public IVector<T> Copy(IVector<T> vector);
}

public abstract class VectorBase<T> : IVector<T> where T : INumberBase<T>
{
    public abstract int Count { get; }
    public abstract T this[int i] { get; set; }
    public abstract double Norm { get; }

    public virtual double ScalarProduct(IVector<T> vector) => ScalarProduct(this, vector);
    public virtual double ScalarProduct() => ScalarProduct(this, this);
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

    protected virtual double ScalarProduct(IVector<T> vectorA, IVector<T> vectorB)
    {
        var result = T.Zero;

        for (var i = 0; i < vectorA.Count; i++)
        {
            result += vectorA[i] * vectorB[i];
        }

        return double.CreateChecked(result);
    }
}


public class Vector<T> : VectorBase<T> where T : INumberBase<T>
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

public static class VectorExtensions
{
    public static IVector<TResult> Sum<TSelf, TOther, TResult>(this IVector<TSelf> vectorA, IVector<TOther> vectorB, IVector<TResult>? result = null)
        where TSelf : INumberBase<TSelf>
        where TOther : INumberBase<TOther>
        where TResult : INumberBase<TResult>
    {
        if (vectorA.Count != vectorB.Count)
            throw new ArgumentOutOfRangeException($"{nameof(vectorA)} and {nameof(vectorB)} must have same size");

        result ??= new Vector<TResult>(vectorA.Count);

        for (var i = 0; i < vectorA.Count; i++)
        {
            result[i] = TResult.CreateChecked(vectorA[i]) + TResult.CreateChecked(vectorB[i]);
        }

        return result;
    }

    public static IVector<TResult> Subtract<TSelf, TOther, TResult>(this IVector<TSelf> vectorA, IVector<TOther> vectorB, IVector<TResult>? result = null)
        where TSelf : INumberBase<TSelf>
        where TOther : INumberBase<TOther>
        where TResult : INumberBase<TResult>
    {
        if (vectorA.Count != vectorB.Count)
            throw new ArgumentOutOfRangeException($"{nameof(vectorA)} and {nameof(vectorB)} must have same size");

        result ??= new Vector<TResult>(vectorA.Count);

        for (var i = 0; i < vectorA.Count; i++)
        {
            result[i] = TResult.CreateChecked(vectorA[i]) - TResult.CreateChecked(vectorB[i]);
        }

        return result;
    }

    public static IVector<TResult> Multiply<TSelf, TCoefficient, TResult>(this IVector<TSelf> vector, TCoefficient number, IVector<TResult>? result = null)
        where TSelf : INumberBase<TSelf>
        where TCoefficient : INumberBase<TCoefficient>
        where TResult : INumberBase<TResult>
    {
        result ??= new Vector<TResult>(vector.Count);

        for (var i = 0; i < vector.Count; i++)
        {
            result[i] = TResult.CreateChecked(number) * TResult.CreateChecked(vector[i]);
        }

        return result;
    }
}