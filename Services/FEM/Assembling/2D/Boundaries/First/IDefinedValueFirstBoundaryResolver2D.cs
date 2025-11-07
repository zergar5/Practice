using Application.FEM._2D;
using Application.FEM.Core.Assembling.Boundaries.First;
using Application.FEM.Core.Grid;
using Domain.Boundaries;
using Domain.Edges;
using Domain.Enums;
using Domain.Nodes;
using System.Numerics;

namespace Application.FEM.Assembling._2D.Boundaries.First;

public interface IDefinedValueFirstBoundaryResolver2D<T> : 
    IDefinedValueFirstBoundaryResolver<T, Edge<Node2D>> where T : INumberBase<T>;