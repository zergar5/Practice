using Application.DirectProblem;
using Application.DirectProblem._2D;
using Application.Extensions;
using Application.FEM.Assembling._2D.Boundaries;
using Application.FEM.Core.Grid;
using Common.Extensions;
using Domain.Edges;
using Domain.Enums;
using Domain.Nodes;

namespace Application.FEM._2D.Assembling.Boundaries;

public class BoundCoverageResolver2D : IBoundCoverageResolver2D
{
    private readonly IDirectProblemContextProvider<DirectProblem2DContext> _problemContextProvider;

    public BoundCoverageResolver2D(IDirectProblemContextProvider<DirectProblem2DContext> problemContextProvider)
    {
        _problemContextProvider = problemContextProvider;
    }

    public int[] ResolveNodeIds(Edge<Node2D> attachment)
    {
        var grid = _problemContextProvider.Get().Grid;
        var edgeType = ResolveBoundType(grid, attachment);

        return edgeType switch
        {
            Bound2D.Lower => ResolveLowerEdgeNodeIds(grid, attachment),
            Bound2D.Left => ResolveLeftEdgeNodeIds(grid, attachment),
            Bound2D.Right => ResolveRightEdgeNodeIds(grid, attachment),
            Bound2D.Upper => ResolveUpperEdgeNodeIds(grid, attachment),
            _ => throw new ArgumentOutOfRangeException()
        };
    }

    private Bound2D ResolveBoundType(IGrid<Node2D, IElement2D> grid, Edge<Node2D> attachment)
    {
        var gridLeftBottomNode = grid.Nodes[0];
        var gridRightUpperNode = grid.Nodes[^1];

        var attachmentBeginNode = attachment.BeginNode;
        var attachmentEndNode = attachment.EndNode;

        if (attachmentBeginNode.Y.Equal(gridLeftBottomNode.Y) &&
            attachmentEndNode.Y.Equal(gridLeftBottomNode.Y))
        {
            return Bound2D.Lower;
        }

        if (attachmentBeginNode.X.Equal(gridLeftBottomNode.X) &&
            attachmentEndNode.X.Equal(gridLeftBottomNode.X))
        {
            return Bound2D.Left;
        }

        if (attachmentBeginNode.X.Equal(gridRightUpperNode.X) &&
            attachmentEndNode.X.Equal(gridRightUpperNode.X))
        {
            return Bound2D.Right;
        }

        if (attachmentBeginNode.Y.Equal(gridRightUpperNode.Y) &&
            attachmentEndNode.Y.Equal(gridRightUpperNode.Y))
        {
            return Bound2D.Upper;
        }

        throw new ArgumentException("Edge is not parallel to any of coordinate axes.");
    }

    private int[] ResolveLowerEdgeNodeIds(IGrid<Node2D, IElement2D> grid, Edge<Node2D> attachment)
    {
        var attachmentBeginNodeId = grid.Nodes.FindIndex(n => n.EqualsWithPrecision(attachment.BeginNode));
        var attachmentEndNodeId = grid.Nodes.FindIndex(n => n.EqualsWithPrecision(attachment.EndNode));

        var resolvedNodeIds = new int[attachmentEndNodeId - attachmentBeginNodeId + 1];

        for (int i = attachmentBeginNodeId, j = 0; i <= attachmentEndNodeId; i++, j++)
        {
            resolvedNodeIds[j] = i;
        }

        return resolvedNodeIds;
    }

    private int[] ResolveLeftEdgeNodeIds(IGrid<Node2D, IElement2D> grid, Edge<Node2D> attachment)
    {
        var widthInNodes = ResolveGridWidthInNodes(grid);

        var attachmentBeginNodeId = 0;
        var attachmentEndNodeId = 0;

        for (var i = 0; i < grid.Nodes.Count / widthInNodes; i++)
        {
            var currentLevelFirstNodeId = i * widthInNodes;
            var nodeFromStart = grid.Nodes[currentLevelFirstNodeId];

            if (nodeFromStart.EqualsWithPrecision(attachment.BeginNode))
            {
                attachmentBeginNodeId = currentLevelFirstNodeId;
                break;
            }
        }

        for (var i = grid.Nodes.Count / widthInNodes; i > 0; i--)
        {
            var currentLevelLastNodeId = (i - 1) * widthInNodes;
            var nodeFromEnd = grid.Nodes[currentLevelLastNodeId];

            if (nodeFromEnd.EqualsWithPrecision(attachment.EndNode))
            {
                attachmentEndNodeId = currentLevelLastNodeId;
                break;
            }
        }

        var resolvedNodeIds = new int[(attachmentEndNodeId - attachmentBeginNodeId) / widthInNodes + 1];

        for (int i = attachmentBeginNodeId, j = 0; i <= attachmentEndNodeId; i += widthInNodes, j++)
        {
            resolvedNodeIds[j] = i;
        }

        return resolvedNodeIds;
    }

    private int[] ResolveRightEdgeNodeIds(IGrid<Node2D, IElement2D> grid, Edge<Node2D> attachment)
    {
        var widthInNodes = ResolveGridWidthInNodes(grid);

        var attachmentBeginNodeId = 0;
        var attachmentEndNodeId = 0;

        for (var i = 0; i < grid.Nodes.Count / widthInNodes; i++)
        {
            var currentLevelFirstNodeId = (i + 1) * widthInNodes - 1;
            var nodeFromStart = grid.Nodes[currentLevelFirstNodeId];

            if (nodeFromStart.EqualsWithPrecision(attachment.BeginNode))
            {
                attachmentBeginNodeId = currentLevelFirstNodeId;
                break;
            }
        }

        for (var i = grid.Nodes.Count / widthInNodes; i > 0; i--)
        {
            var currentLevelFirstNodeId = i * widthInNodes - 1;
            var nodeFromEnd = grid.Nodes[currentLevelFirstNodeId];

            if (nodeFromEnd.EqualsWithPrecision(attachment.EndNode))
            {
                attachmentEndNodeId = currentLevelFirstNodeId;
                break;
            }
        }

        var resolvedNodeIds = new int[(attachmentEndNodeId - attachmentBeginNodeId) / widthInNodes + 1];

        for (int i = attachmentBeginNodeId, j = 0; i <= attachmentEndNodeId; i += widthInNodes, j++)
        {
            resolvedNodeIds[j] = i;
        }

        return resolvedNodeIds;
    }

    private int[] ResolveUpperEdgeNodeIds(IGrid<Node2D, IElement2D> grid, Edge<Node2D> attachment)
    {
        var attachmentBeginNodeId = grid.Nodes.FindLastIndex(n => n.EqualsWithPrecision(attachment.BeginNode));
        var attachmentEndNodeId = grid.Nodes.FindLastIndex(n => n.EqualsWithPrecision(attachment.EndNode));

        var resolvedNodeIds = new int[attachmentEndNodeId - attachmentBeginNodeId + 1];

        for (int i = attachmentBeginNodeId, j = 0; i <= attachmentEndNodeId; i++, j++)
        {
            resolvedNodeIds[j] = i;
        }

        return resolvedNodeIds;
    }

    private int ResolveGridWidthInNodes(IGrid<Node2D, IElement2D> grid)
    {
        var gridLeftBottomNode = grid.Nodes[0];

        return grid.Nodes.FindIndex(n => n.X.Equal(gridLeftBottomNode.X) && !n.Y.Equal(gridLeftBottomNode.Y));
    }
}