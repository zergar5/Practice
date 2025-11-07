using Application.FEM.Core.Assembling;
using Application.FEM.Core.Grid;
using Domain.Nodes;

namespace Application.FEM.Assembling.PortraitBuilders;

public class HarmonicMatrixPortraitBuilder<TNode, TElement> : MatrixPortraitBuilderBase<TNode, TElement>
    where TNode : Node
    where TElement : IElement
{
    protected override List<SortedSet<int>> BuildAdjacencyList(IGrid<TNode, TElement> grid)
    {
        if (AdjacencyList.Count != grid.Nodes.Count * 2)
        {
            AdjacencyList = new List<SortedSet<int>>(grid.Nodes.Count * 2);

            for (var i = 0; i < grid.Nodes.Count * 2; i++)
            {
                AdjacencyList.Add([]);
            }
        }

        foreach (var element in grid)
        {
            var nodesIndexes = element.NodeIndexes;

            foreach (var currentNode in nodesIndexes)
            {
                for (var i = 0; i < 2; i++)
                {
                    var currentComplexNode = currentNode * 2 + i;

                    foreach (var nodeIndex in nodesIndexes)
                    {
                        for (var j = 0; j < 2; j++)
                        {
                            var complexNodeIndex = nodeIndex * 2 + j;
                            if (currentComplexNode > complexNodeIndex)
                                AdjacencyList[currentComplexNode].Add(complexNodeIndex);
                        }
                    }
                }
            }
        }

        return AdjacencyList;
    }
}