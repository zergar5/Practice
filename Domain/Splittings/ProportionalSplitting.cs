namespace Domain.Splittings;

public interface IProportionalSplitting : IUniformSplitting
{
    public double DischargeRatio { get; set; }
}

public class ProportionalSplitting : IProportionalSplitting
{
    public required IInterval Interval { get; set; }
    public double Step { get; set; }
    public double DischargeRatio { get; set; }
}