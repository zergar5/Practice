using Application.FEM.Core;
using Application.FEM.Core.Grid;
using Domain.Nodes;

namespace Application.FEM.Assembling.PortraitBuilders;

public class HarmonicMatrixPortraitBuilder : MatrixPortraitBuilder
{
    protected override List<SortedSet<int>> BuildAdjacencyList(IGrid<Node> grid)
    {
        var adjacencyList = new List<SortedSet<int>>(grid.Nodes.Length * 2);

        for (var i = 0; i < grid.Nodes.Length * 2; i++)
        {
            adjacencyList.Add([]);
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
                                adjacencyList[currentComplexNode].Add(complexNodeIndex);
                        }
                    }
                }
            }
        }

        return adjacencyList;
    }
}