using Domain.Enums;

namespace Domain.Boundaries;

public interface IBoundaryCondition
{
    public string Expression { get; set; }
    public BoundaryConditionType Type { get; set; }
}

public class BoundaryCondition : IBoundaryCondition
{
    public required string Expression { get; set; }
    public BoundaryConditionType Type { get; set; }
}