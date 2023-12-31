﻿using Practice6Sem.GridGenerator.Intervals.Core;

namespace Practice6Sem.GridGenerator.Intervals.Splitting;

public readonly record struct UniformSplitter(int Steps) : IIntervalSplitter
{
    public IEnumerable<double> EnumerateValues(Interval interval)
    {
        var step = interval.Length / Steps;

        for (var stepNumber = 0; stepNumber <= Steps; stepNumber++)
        {
            var value = interval.Begin + stepNumber * step;

            yield return value;
        }
    }
}