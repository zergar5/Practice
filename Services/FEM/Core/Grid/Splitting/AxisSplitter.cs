using Domain;

namespace Application.FEM.Core.Grid.Splitting;

public interface IAxisSplitter
{
    public IEnumerable<double> SplitAxis(IEnumerable<double> controlPoints, params ISplitStrategy[] splitStrategies);
}

public class AxisSplitter : IAxisSplitter
{
    public IEnumerable<double> SplitAxis(IEnumerable<double> controlPoints, params ISplitStrategy[] splitStrategies)
    {
        if (controlPoints.Count() - 1 != splitStrategies.Length)
            throw new ArgumentException("Incorrect number of control points or splitters");

        var intervals = BuildIntervals(controlPoints).ToArray();
        foreach (var value in splitStrategies[0].ExecuteSplit(intervals[0]))
        {
            yield return value;
        }

        for (var i = 1; i < splitStrategies.Length; i++)
        {
            foreach (var value in splitStrategies[i].ExecuteSplit(intervals[i]).Skip(1))
            {
                yield return value;
            }
        }
    }

    private IEnumerable<Interval> BuildIntervals(IEnumerable<double> points)
    {
        var begin = points.First();
        foreach (var point in points.Skip(1))
        {
            var end = point;
            yield return new Interval { Begin = begin, End = end };

            begin = end;
        }
    }
}