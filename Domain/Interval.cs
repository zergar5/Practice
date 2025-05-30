namespace Domain;

public interface IInterval
{
    public double Begin { get; set; }
    public double End { get; set; }
}

public class Interval : IInterval
{
    public double Begin { get; set; }
    public double End { get; set; }
}