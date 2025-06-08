using System.Numerics;

namespace Application.FEM.Core.Assembling.Boundaries.First;

public interface IFirstBoundaryValue<out T> where T : INumberBase<T>
{
    public int NodeId { get; }
    public T Value { get; }
}

public class FirstBoundaryValue<T> : IFirstBoundaryValue<T> where T : INumberBase<T>
{
    public int NodeId { get; }
    public T Value { get; }

    public FirstBoundaryValue(T value, int nodeId)
    {
        Value = value;
        NodeId = nodeId;
    }
}