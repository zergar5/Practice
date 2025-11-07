using Application.FEM.Core.Grid;
using System.Numerics;

namespace Application.FEM.Core.BasisFunctions;

public interface IBasisFunctionsProvider<in TNode, out T, in TElement>
    where T : INumberBase<T>
    where TElement : IElement
{
    public IBasisFunction<TNode, T>[] GetFunctions(TElement element);
}