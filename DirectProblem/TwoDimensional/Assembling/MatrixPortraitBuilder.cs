using DirectProblem.Core;
using DirectProblem.Core.Global;
using DirectProblem.Core.GridComponents;
using DirectProblem.FEM.Assembling;

namespace DirectProblem.TwoDimensional.Assembling;

public class MatrixPortraitBuilder : IMatrixPortraitBuilder<Node2D, SparseMatrix>
{
    private List<SortedSet<int>> _adjacencyList = null!;

    public SparseMatrix Build(Grid<Node2D> grid)
    {
        BuildAdjacencyList(grid);

        var amount = 0;
        var rowsIndexes = new[] { 0 };
        rowsIndexes = rowsIndexes.Concat(_adjacencyList.Select(nodeSet => amount += nodeSet.Count)).ToArray();

        var columnsIndexes = _adjacencyList.SelectMany(nodeList => nodeList).ToArray();

        return new SparseMatrix(rowsIndexes, columnsIndexes);
    }

    private void BuildAdjacencyList(Grid<Node2D> grid)
    {
        _adjacencyList = new List<SortedSet<int>>(grid.Nodes.Length * 2);

        for (var i = 0; i < grid.Nodes.Length * 2; i++)
        {
            _adjacencyList.Add(new SortedSet<int>());
        }

        foreach (var element in grid)
        {
            var nodesIndexes = element.NodesIndexes;

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
                                _adjacencyList[currentComplexNode].Add(complexNodeIndex);
                        }
                    }
                }
            }
        }
    }
}