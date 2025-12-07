using Application.FEM.Core.Grid;

namespace Application.FEM._1D;

public class Element : ElementBase
{
    private readonly int[] _nodeIndexes;

    public override IReadOnlyList<int> NodeIndexes => _nodeIndexes.AsReadOnly();

    public Element(int materialId, int[] nodeIndexes, double length) : base(materialId, length)
    {
        _nodeIndexes = nodeIndexes;
    }
}