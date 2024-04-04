namespace DirectProblem.GridGenerator.Intervals.Core;

public readonly record struct Interval(double Begin, double End)
{
    public double Length => Math.Abs(End - Begin);

    public bool Has(double value)
    {
        return value >= Begin && value <= End;
    }
}