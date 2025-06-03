using System.Numerics;
using Application.FEM.Core;
using Domain.Nodes;
using Services.MathObjects.Matrices;

namespace Application.FEM.Assembling.PortraitBuilders;

public interface IMatrixPortraitBuilder
{
    ISparseMatrix<T> Build<T>(IGrid<INode> grid) where T : INumber<T>;
}

public abstract class MatrixPortraitBuilder : IMatrixPortraitBuilder
{
    public virtual ISparseMatrix<T> Build<T>(IGrid<INode> grid) where T : INumber<T>
    {
        var adjacencyList = BuildAdjacencyList(grid);

        var amount = 0;
        var rowsIndexes = adjacencyList.Select(nodeSet => amount += nodeSet.Count).Prepend(0).ToArray();
        var columnsIndexes = adjacencyList.SelectMany(nodeList => nodeList).ToArray();

        return new SparseMatrix<T>(rowsIndexes, columnsIndexes);
    }

    protected abstract List<SortedSet<int>> BuildAdjacencyList(IGrid<INode> grid);
}