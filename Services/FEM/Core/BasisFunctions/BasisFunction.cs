using System.Numerics;

namespace Application.FEM.Core.BasisFunctions;

public interface IBasisFunction<in TNode, out T> where T : INumberBase<T>
{
    public T Evaluate(TNode node);
}