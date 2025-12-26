using Common.Extensions;

namespace Domain;

public class Interval
{
    public double Begin { get; set; }
    public double End { get; set; }
}

public static class IntervalExtensions
{
    public static double Length(this Interval interval) => interval.End - interval.Begin;
    public static bool Has(this Interval interval, double point) => interval.Begin.LessOrEqualThan(point) && point.LessOrEqualThan(interval.End);
}