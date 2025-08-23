namespace Domain.Areas;

public interface IArea
{
    public int MaterialId { get; set; }
}

public class Area : IArea
{
    public int MaterialId { get; set; }
    public int LeftControlPointId { get; set; }
    public int RightControlPointId { get; set; }
}
