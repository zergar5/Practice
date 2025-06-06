using Application.FEM.Core.Grid;
using System.Collections.Immutable;

namespace Application.FEM._1D;

public class Element : ElementBase
{
    private readonly int[] _nodeIndexes;

    public override ImmutableArray<int> NodeIndexes => [.. _nodeIndexes];

    public Element(int materialId, int[] nodeIndexes, double length) : base(materialId, length)
    {
        _nodeIndexes = nodeIndexes;
    }
}