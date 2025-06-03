using System.Collections;
using System.Collections.Immutable;
using Domain.Nodes;

namespace Application.FEM.Core;

public interface IGrid<TNode> : IEnumerable<IElement> where TNode : INode
{
    public ImmutableArray<TNode> Nodes { get; }
    public ImmutableArray<IElement> Elements { get; }
}

public class Grid<TNode> : IGrid<TNode> where TNode : INode
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