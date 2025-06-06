using Domain;
using Interval = Domain.Interval;

namespace Application.FEM.Core.Grid.Splitting;

public class UniformSplitStrategy : ISplitStrategy
{
    private readonly int _steps;

    public UniformSplitStrategy(int steps)
    {
        _steps = steps;
    }

    public IEnumerable<double> ExecuteSplit(Interval interval)
    {
        var step = interval.Length() / _steps;

        var stepNumber = 0;
        var value = interval.Begin + stepNumber * step;

        while (interval.Has(value))
        {
            yield return value;
            stepNumber++;
            value = interval.Begin + stepNumber * step;
        }
    }
}

public class StepUniformSplitStrategy : ISplitStrategy
{
    private readonly double _step;
    private UniformSplitStrategy? _uniformSplitStrategy;

    public StepUniformSplitStrategy(double step)
    {
        _step = step;
    }

    public IEnumerable<double> ExecuteSplit(Interval interval)
    {
        var steps = interval.Length() / _step;
        // заменить 1e-15 на глобальную точность для чисел double
        if (!(Math.Abs(steps % _step) <= 1e-15))
        {
            throw new ArgumentException("Invalid step or interval");
        }

        _uniformSplitStrategy ??= new UniformSplitStrategy((int)Math.Round(steps));

        var values = _uniformSplitStrategy.ExecuteSplit(interval);

        foreach (var value in values)
        {
            yield return value;
        }
    }
}