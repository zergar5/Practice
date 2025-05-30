namespace Domain.Splittings;

public interface IProportionalSplitter : IUniformSplitting
{
    public double DischargeRatio { get; set; }
}

public class ProportionalSplitter : IProportionalSplitter
{
    public required IInterval Interval { get; set; }
    public double Step { get; set; }
    public double DischargeRatio { get; set; }
}