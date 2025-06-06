using System.Collections;
using System.Collections.Immutable;

namespace Application.FEM.Core.Grid;

public interface IGrid<TNode> : IEnumerable<IElement>
{
    public ImmutableArray<TNode> Nodes { get; }
    public ImmutableArray<IElement> Elements { get; }
}

public class Grid<TNode> : IGrid<TNode>
{
    private readonly TNode[] _nodes;
    private readonly IElement[] _elements;

    public ImmutableArray<TNode> Nodes => [.. _nodes];
    public ImmutableArray<IElement> Elements => [.. _elements];

    public Grid(TNode[] nodes, IElement[] elements)
    {
        _nodes = nodes;
        _elements = elements;
    }

    IEnumerator IEnumerable.GetEnumerator()
    {
        return GetEnumerator();
    }

    public IEnumerator<IElement> GetEnumerator() => ((IEnumerable<IElement>)Elements).GetEnumerator();
}