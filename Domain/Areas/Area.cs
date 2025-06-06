namespace Domain.Areas;

public interface IArea
{
    public int MaterialId { get; set; }
}

public class Area : IArea
{
    public int MaterialId { get; set; }
    public int BeginIndex { get; set; }
    public int EndIndex { get; set; }
}
