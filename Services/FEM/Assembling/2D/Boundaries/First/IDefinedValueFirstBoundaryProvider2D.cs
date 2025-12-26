using Application.FEM._2D.Grid;
using Domain.Boundaries;
using Domain.Edges;
using Domain.Nodes;
using System.Numerics;

namespace Application.FEM.Assembling._2D.Boundaries.First;

public interface IDefinedValueFirstBoundaryProvider2D<T> where T : INumberBase<T>
{
    public IDefinedValueBoundaryCondition<Edge<Node2D>, T>[] GetOnAllBounds(GridBuilder2D.Grid2DParameters grid2DParameters, T boundaryValue);
}