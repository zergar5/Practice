using Domain.Boundaries;
using System.Numerics;

namespace Application.FEM.Core.Assembling.Boundaries.First;

public interface IFirstBoundaryResolver<out T, TAttachment> where T : INumberBase<T>
{
    public IFirstBoundaryValue<T>[] ResolveBoundaryValues(IBoundRaw<TAttachment> bound);
}