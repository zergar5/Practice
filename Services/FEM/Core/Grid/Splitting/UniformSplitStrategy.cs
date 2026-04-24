using Common.Extensions;
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
    private readonly bool _safe;
    private UniformSplitStrategy _uniformSplitStrategy;

    public StepUniformSplitStrategy(double step, bool safe = false)
    {
        _step = step;
        _safe = safe;
    }

    public IEnumerable<double> ExecuteSplit(Interval interval)
    {
        var steps = interval.Length() / _step;

        if (_safe)
        {
            steps = Math.Round(steps, MidpointRounding.AwayFromZero);
        }
        else
        {
            if (!Math.Abs(steps % 1).EqualTo(0))
            {
                throw new ArgumentException("Invalid step or interval");
            }
        }

        _uniformSplitStrategy = new UniformSplitStrategy((int)steps);

        var values = _uniformSplitStrategy.ExecuteSplit(interval);

        foreach (var value in values)
        {
            yield return value;
        }
    }
}