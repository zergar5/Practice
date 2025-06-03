using Domain.Enums;

namespace Domain.Boundaries;

public interface IBoundRaw<TAttachment, TCondition>
{
    public TAttachment Attachment { get; set; }
    public BoundaryConditionType ConditionType { get; set; }
    public TCondition Condition { get; set; }
}

//public class BoundRawWithFirstCondition<TAttachment> : IBoundRaw<IBoundaryCondition>
//{
//    public BoundaryConditionType ConditionType { get; set; }
//    public IBoundaryCondition Condition { get; set; }
//}