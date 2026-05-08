using Domain;

namespace Application.FEM.Core.Grid.Splitting;

public class ProportionalSplitStrategy : ISplitStrategy
{
    private readonly int _steps;
    private readonly double _dischargeRatio;

    public ProportionalSplitStrategy(int steps, double dischargeRatio)
    {
        if (Math.Abs(dischargeRatio - 1d) < 1e-15)
            throw new NotSupportedException();

        _steps = steps;
        _dischargeRatio = dischargeRatio;
    }

    public IEnumerable<double> ExecuteSplit(Interval interval)
    {
        var step = interval.Length() * (_dischargeRatio - 1d) / (Math.Pow(_dischargeRatio, _steps) - 1d);
        var stepNumber = 0;
        var value = interval.Begin;

        while (interval.Has(value))
        {
            yield return value;

            var nextValue = interval.Begin + step * (Math.Pow(_dischargeRatio, stepNumber + 1) - 1d) / (_dischargeRatio - 1d);

            value = nextValue;
            stepNumber++;
        }
    }
}

public class StepProportionalSplitStrategy : ISplitStrategy
{
    private readonly double _startStep;
    private readonly double _dischargeRatio;

    public StepProportionalSplitStrategy(double startStep, double dischargeRatio)
    {
        if (Math.Abs(dischargeRatio - 1d) < 1e-15)
            throw new NotSupportedException();

        _startStep = startStep;
        _dischargeRatio = dischargeRatio;
    }

    public IEnumerable<double> ExecuteSplit(Interval interval)
    {
        var values = new List<double>();
        var stepNumber = 0;
        double value;

        if (_dischargeRatio < 1)
        {
            value = interval.End;

            var dischargeRatio = 1 / _dischargeRatio;

            while (interval.Has(value))
            {
                values.Add(value);

                var nextValue = value - _startStep * Math.Pow(dischargeRatio, stepNumber);

                value = nextValue;
                stepNumber++;

                if (!(interval.Begin > value - _startStep * Math.Pow(dischargeRatio, stepNumber))) continue;

                values.Add(interval.Begin);

                break;
            }

            values.Reverse();
        }
        else
        {
            value = interval.Begin;

            while (interval.Has(value))
            {
                values.Add(value);

                var nextValue = value + _startStep * Math.Pow(_dischargeRatio, stepNumber);

                value = nextValue;
                stepNumber++;

                if (!(interval.End < value + _startStep * Math.Pow(_dischargeRatio, stepNumber))) continue;

                values.Add(interval.End);

                break;
            }
        }

        foreach (var pointValue in values)
        {
            yield return pointValue;
        }
    }
}