using System.Numerics;
using Domain.Nodes;

namespace Application.FEM.Core.BasisFunctions;

public interface IBasisFunctionsProvider<in TNode, in TElement>
    where TNode : INode
    where TElement : IElement
{
    public IBasisFunction<TNode, TFunctionResult>[] GetFunctions<TFunctionResult>(TElement element)
        where TFunctionResult : INumber<TFunctionResult>;
}