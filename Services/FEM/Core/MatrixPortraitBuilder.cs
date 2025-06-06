using Application.FEM.Core.Grid;
using Domain.Nodes;
using Services.MathObjects.Matrices;
using System.Numerics;

namespace Application.FEM.Core;

public interface IMatrixPortraitBuilder
{
    ISparseMatrix<T> Build<T>(IGrid<Node> grid) where T : INumber<T>;
}

public abstract class MatrixPortraitBuilder : IMatrixPortraitBuilder
{
    public virtual ISparseMatrix<T> Build<T>(IGrid<Node> grid) where T : INumber<T>
    {
        var adjacencyList = BuildAdjacencyList(grid);

        var amount = 0;
        var rowsIndexes = adjacencyList.Select(nodeSet => amount += nodeSet.Count).Prepend(0).ToArray();
        var columnsIndexes = adjacencyList.SelectMany(nodeList => nodeList).ToArray();

        return new SparseMatrix<T>(rowsIndexes, columnsIndexes);
    }

    protected abstract List<SortedSet<int>> BuildAdjacencyList(IGrid<Node> grid);
}