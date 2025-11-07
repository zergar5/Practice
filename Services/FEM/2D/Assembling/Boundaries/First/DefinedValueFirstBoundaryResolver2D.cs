using System.Numerics;
using Application.FEM.Core.Assembling.Boundaries.First;
using Domain.Boundaries;
using Domain.Edges;
using Domain.Enums;
using Domain.Nodes;

namespace Application.FEM.Assembling._2D.Boundaries.First;

public class DefinedValueFirstBoundaryResolver2D<T> : IDefinedValueFirstBoundaryResolver2D<T> where T : INumberBase<T>
{
    private readonly IBoundCoverageResolver2D _boundCoverageResolver2D;

    public DefinedValueFirstBoundaryResolver2D(IBoundCoverageResolver2D boundCoverageResolver2D)
    {
        _boundCoverageResolver2D = boundCoverageResolver2D;
    }

    public IFirstBoundaryValue<T>[] ResolveBoundaryValues(IDefinedValueBoundaryCondition<Edge<Node2D>, T> boundary)
    {
        if (boundary.Type != BoundaryConditionType.First)
            throw new ArgumentException("Input boundary not first");

        var boundaryEdgeNodeIds = _boundCoverageResolver2D.ResolveNodeIds(boundary.Attachment);
        var firstBoundaryValues = new IFirstBoundaryValue<T>[boundaryEdgeNodeIds.Length];

        for (var i = 0; i < firstBoundaryValues.Length; i++)
        {
            firstBoundaryValues[i] = new FirstBoundaryValue<T>(boundary.Value, boundaryEdgeNodeIds[i]);
        }

        return firstBoundaryValues;
    }
}