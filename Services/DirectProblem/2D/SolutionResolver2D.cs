using Domain.Nodes;
using System.Numerics;

namespace Application.DirectProblem._2D;

public interface ISolutionResolver2D<out T> : ISolutionResolver<T, Node2D> where T : INumberBase<T>;