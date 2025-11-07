using Domain.Enums;
using Domain.Interfaces;

namespace Domain.Boundaries;

public interface IBoundaryConditionExpression<TAttachment> : IAttached<TAttachment>
{
    public string Expression { get; set; }
    public BoundaryConditionType Type { get; set; }
}