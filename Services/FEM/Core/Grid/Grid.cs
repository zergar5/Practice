using Application.FEM._2D;
using Common.Extensions;
using Domain.Nodes;
using System.Collections;

namespace Application.FEM.Core.Grid;

public interface IGrid<out TNode, out TElement> : IEnumerable<TElement>
    where TElement : IElement
{
    public IReadOnlyList<TNode> Nodes { get; }
    public IReadOnlyList<TElement> Elements { get; }
}

public class Grid<TNode, TElement> : IGrid<TNode, TElement>
    where TElement : IElement
{
    private readonly TNode[] _nodes;
    private readonly TElement[] _elements;

    public IReadOnlyList<TNode> Nodes => _nodes.AsReadOnly();
    public IReadOnlyList<TElement> Elements => _elements.AsReadOnly();

    public Grid(TNode[] nodes, TElement[] elements)
    {
        _nodes = nodes;
        _elements = elements;
    }

    IEnumerator IEnumerable.GetEnumerator()
    {
        return GetEnumerator();
    }

    public IEnumerator<TElement> GetEnumerator() => Elements.GetEnumerator();
}

public static class GridExtensions
{
    public static bool Has(this IGrid<Node2D, IElement2D> grid, Node2D node)
    {
        var lowerLeftCorner = grid.Nodes[0];
        var upperRightCorner = grid.Nodes[^1];
        return PointInRectangle(node, lowerLeftCorner, upperRightCorner);
    }

    public static IElement2D? FindNodeElement(this IGrid<Node2D, IElement2D> grid, Node2D node)
    {
        return grid.Elements.FirstOrDefault(e => PointInRectangle(node, grid.Nodes[e.NodeIndexes[0]], grid.Nodes[e.NodeIndexes[^1]]));
    }

    // TODO вынести потом в другой место, связанное с геометрией, например завести class Rectangle
    public static bool PointInRectangle(Node2D node, Node2D lowerLeftCorner, Node2D upperRightCorner)
    {
        return node.X.GreaterOrEqualThan(lowerLeftCorner.X) && node.Y.GreaterOrEqualThan(lowerLeftCorner.Y) &&
               node.X.LessOrEqualThan(upperRightCorner.X) && node.Y.LessOrEqualThan(upperRightCorner.Y);
    }
}