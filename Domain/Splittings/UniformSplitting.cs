namespace Domain.Splittings;

public class UniformSplitting
{
    public required Interval Interval { get; set; }
    public int Steps { get; set; }
}

public class StepUniformSplitting
{
    public required Interval Interval { get; set; }
    public double Step { get; set; }
}