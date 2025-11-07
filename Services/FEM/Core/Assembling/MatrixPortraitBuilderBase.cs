using Application.FEM.Core.Grid;
using Application.MathObjects.Matrices;
using Domain.Nodes;
using System.Numerics;

namespace Application.FEM.Core.Assembling;

public interface IMatrixPortraitBuilder<in TNode, in TElement>
    where TNode : Node
    where TElement : IElement
{
    ISparseMatrix<T> Build<T>(IGrid<TNode, TElement> grid) where T : INumberBase<T>;
}

public abstract class MatrixPortraitBuilderBase<TNode, TElement> : IMatrixPortraitBuilder<TNode, TElement>
    where TNode : Node
    where TElement : IElement
{
    protected List<SortedSet<int>> AdjacencyList = [];

    public virtual ISparseMatrix<T> Build<T>(IGrid<TNode, TElement> grid) where T : INumberBase<T>
    {
        AdjacencyList = BuildAdjacencyList(grid);

        var amount = 0;
        var rowsIndexes = AdjacencyList.Select(nodeSet => amount += nodeSet.Count).Prepend(0).ToArray();
        var columnsIndexes = AdjacencyList.SelectMany(nodeList => nodeList).ToArray();

        return new SparseMatrix<T>(rowsIndexes, columnsIndexes);
    }

    protected abstract List<SortedSet<int>> BuildAdjacencyList(IGrid<TNode, TElement> grid);
}