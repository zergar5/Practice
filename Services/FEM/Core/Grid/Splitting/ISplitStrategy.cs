using Domain;

namespace Application.FEM.Core.Grid.Splitting;

public interface ISplitStrategy
{
    public IEnumerable<double> ExecuteSplit(Interval interval);
}