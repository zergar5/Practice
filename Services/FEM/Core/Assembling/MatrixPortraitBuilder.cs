using Application.FEM.Core.Grid;
using Application.MathObjects.Matrices;
using Domain.Nodes;
using System.Numerics;

namespace Application.FEM.Core.Assembling;

public interface IMatrixPortraitBuilder
{
    ISparseMatrix<T> Build<T>(IGrid<Node> grid) where T : INumberBase<T>;
}

public abstract class MatrixPortraitBuilder : IMatrixPortraitBuilder
{
    public virtual ISparseMatrix<T> Build<T>(IGrid<Node> grid) where T : INumberBase<T>
    {
        var adjacencyList = BuildAdjacencyList(grid);

        var amount = 0;
        var rowsIndexes = adjacencyList.Select(nodeSet => amount += nodeSet.Count).Prepend(0).ToArray();
        var columnsIndexes = adjacencyList.SelectMany(nodeList => nodeList).ToArray();

        return new SparseMatrix<T>(rowsIndexes, columnsIndexes);
    }

    protected abstract List<SortedSet<int>> BuildAdjacencyList(IGrid<Node> grid);
}