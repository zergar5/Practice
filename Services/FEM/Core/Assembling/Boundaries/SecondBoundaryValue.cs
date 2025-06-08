using System.Numerics;

namespace Application.FEM.Core.Assembling.Boundaries;

public interface ISecondBoundaryValue<out T> where T : INumberBase<T>
{
    public int[] NodeIds { get; }
    public T[] Values { get; }
}

public class SecondBoundaryValue<T> : ISecondBoundaryValue<T> where T : INumberBase<T>
{
    public int[] NodeIds { get; }
    public T[] Values { get; }

    public SecondBoundaryValue(int[] nodeIds, T[] values)
    {
        NodeIds = nodeIds;
        Values = values;
    }
}