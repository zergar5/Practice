using Application.FEM.Core.Assembling.Boundaries.First;
using Domain.Boundaries;
using Domain.Edges;
using System.Numerics;

namespace Application.FEM.Assembling._2D.Boundaries.First;

public interface IFirstBoundaryResolver2D<out T> : IFirstBoundaryResolver<T, Edge> where T : INumberBase<T>;

public class FirstBoundaryResolver2D<T> : IFirstBoundaryResolver2D<T> where T : INumberBase<T>
{
    public IFirstBoundaryValue<T>[] ResolveBoundaryValues(IBoundRaw<Edge> bound)
    {
        throw new NotImplementedException();
    }
}