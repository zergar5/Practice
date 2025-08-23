namespace Domain.Areas;

public class Area2D : IArea
{
    public int MaterialId { get; set; }
    public int LeftLowerControlPointId { get; set; }
    public int RightLowerControlPointId { get; set; }
    public int LeftUpperControlPointId { get; set; }
    public int RightUpperControlPointId { get; set; }
}