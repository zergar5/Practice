using System.Collections.Immutable;

namespace Application.FEM.Core.Grid;

public interface IElement
{
    public int MaterialId { get; }
    public ImmutableArray<int> NodeIndexes { get; }
    public double Length { get; }
}

public abstract class ElementBase : IElement
{
    public virtual int MaterialId { get; }
    public abstract ImmutableArray<int> NodeIndexes { get; }
    public virtual double Length { get; }

    protected ElementBase(int materialId, double length)
    {
        MaterialId = materialId;
        Length = length;
    }
}