using Domain.Nodes;

namespace Application.FEM.Core.Grid;

public interface IGridBuilder<out TNode, out TElement, in TGridParameters>
    where TNode : Node
    where TElement : IElement
{
    public IGrid<TNode, TElement> Build(TGridParameters gridParameters);
}