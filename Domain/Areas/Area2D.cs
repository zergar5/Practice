namespace Domain.Areas;

public class Area2D : IArea
{
    public int MaterialId { get; set; }
    public int XStartControlPointId { get; set; }
    public int XEndControlPointId { get; set; }
    public int YStartControlPointId { get; set; }
    public int YEndControlPointId { get; set; }
}