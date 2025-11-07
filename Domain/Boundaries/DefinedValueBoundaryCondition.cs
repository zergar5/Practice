using Domain.Edges;
using Domain.Enums;
using Domain.Interfaces;
using System.Numerics;

namespace Domain.Boundaries;

public interface IDefinedValueBoundaryCondition<TAttachment, TValue> : IAttached<TAttachment>
    where TValue : INumberBase<TValue>
{
    public TValue Value { get; set; }
    public BoundaryConditionType Type { get; set; }
}

public class EdgeBound<TNode, TValue> : IDefinedValueBoundaryCondition<Edge<TNode>, TValue>
    where TValue : INumberBase<TValue>
{
    public required Edge<TNode> Attachment { get; set; }
    public required TValue Value { get; set; }
    public BoundaryConditionType Type { get; set; }
}