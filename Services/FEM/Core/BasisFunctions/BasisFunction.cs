using System.Numerics;
using Domain.Nodes;

namespace Application.FEM.Core.BasisFunctions;

public interface IBasisFunction<in TNode, out TResult>
    where TNode : INode
    where TResult : INumber<TResult>
{
    public TResult Evaluate(TNode node);
}