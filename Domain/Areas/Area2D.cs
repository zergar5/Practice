namespace Domain.Areas;

public class Area2D : IArea
{
    public int MaterialId { get; set; }
    public int BeginXControlPointId { get; set; }
    public int BeginYControlPointId { get; set; }
    public int EndXControlPointId { get; set; }
    public int EndYControlPointId { get; set; }
}