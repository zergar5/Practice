using Domain.Nodes;

namespace Application.FEM.Core.Grid;

public interface IGridBuilder<TNode, in TGridParameters> where TNode : Node
{
    public Grid<TNode> Build(TGridParameters gridParameters);
}