namespace Domain.Splittings;

public interface IUniformSplitting
{
    public IInterval Interval { get; set; }
    public double Step { get; set; }
}

public class UniformSplitting : IUniformSplitting
{
    public required IInterval Interval { get; set; }
    public double Step { get; set; }
}