using Application.Extensions;
using Application.FEM._2D;
using Application.FEM.Core.Assembling.Boundaries;
using Application.FEM.Core.Grid;
using Common.Extensions;
using Domain.Edges;
using Domain.Enums;
using Domain.Nodes;

namespace Application.FEM.Assembling._2D.Boundaries;

public interface IBoundCoverageResolver2D : IBoundCoverageResolver<Edge<Node2D>>;