using System.Collections.Immutable;

namespace Application.FEM.Core;

public interface IElement
{
    public ImmutableArray<int> NodeIndexes { get; }
}

public abstract class Element : IElement
{
    public abstract ImmutableArray<int> NodeIndexes { get; }
}