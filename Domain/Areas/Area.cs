namespace Domain.Areas;

public interface IArea
{
    public int MaterialId { get; set; }
}

public class Area : IArea
{
    public int MaterialId { get; set; }
    public int BeginId { get; set; }
    public int EndId { get; set; }
}
