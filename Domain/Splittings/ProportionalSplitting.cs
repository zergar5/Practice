namespace Domain.Splittings;

public class ProportionalSplitting
{
    public required Interval Interval { get; set; }
    public int Steps { get; set; }
    public double DischargeRatio { get; set; }
}

public class StepProportionalSplitting
{
    public required Interval Interval { get; set; }
    public double Step { get; set; }
    public double DischargeRatio { get; set; }
}