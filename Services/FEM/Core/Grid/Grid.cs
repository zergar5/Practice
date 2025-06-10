using Domain.Nodes;
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

public static class GridExtensions
{
    public static bool Has(this IGrid<Node2D> grid, Node2D node)
    {
        var lowerLeftCorner = grid.Nodes[0];
        var upperRightCorner = grid.Nodes[^1];
        return PointInRectangle(node, lowerLeftCorner, upperRightCorner);
    }

    public static IElement? FindNodeElement(this IGrid<Node2D> grid, Node2D node)
    {
        return grid.Elements.FirstOrDefault(e => PointInRectangle(node, grid.Nodes[e.NodeIndexes[0]], grid.Nodes[e.NodeIndexes[^1]]));
    }

    // TODO вынести потом в другой место, связанное с геометрией, например завести class Rectangle
    public static bool PointInRectangle(Node2D node, Node2D lowerLeftCorner, Node2D upperRightCorner)
    {
        return node.X > lowerLeftCorner.X && node.Y > lowerLeftCorner.Y &&
               node.X < upperRightCorner.X && node.Y < upperRightCorner.Y;
    }
}