using Application.FEM.Core.Assembling.Boundaries;
using Domain.Edges;
using Domain.Nodes;

namespace Application.FEM.Assembling._2D.Boundaries;

public interface IBoundCoverageResolver2D : IBoundCoverageResolver<Edge<Node2D>>;