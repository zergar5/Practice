using Practice6Sem.GridGenerator.Intervals.Core;

namespace Practice6Sem.GridGenerator.Intervals.Splitting;

public interface IIntervalSplitter
{
    public IEnumerable<double> EnumerateValues(Interval interval);
    public int Steps { get; }
}