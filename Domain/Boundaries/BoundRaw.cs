using Domain.Edges;
using Domain.Enums;

namespace Domain.Boundaries;

public interface IBoundRaw<TAttachment>
{
    public TAttachment Attachment { get; set; }
    public IBoundaryCondition Condition { get; set; }
}

public class ControlPointBound : IBoundRaw<int>
{
    public int Attachment { get; set; }
    public required IBoundaryCondition Condition { get; set; }
}

public class EdgeBound : IBoundRaw<Edge>
{
    public required Edge Attachment { get; set; }
    public required IBoundaryCondition Condition { get; set; }
}