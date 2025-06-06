using Application.FEM.Core.Grid;

namespace Application.FEM.Core.BasisFunctions;

public interface IBasisFunctionsProvider<in TNode, in TElement> where TElement : IElement
{
    public IBasisFunction<TNode>[] GetFunctions(TElement element);
}