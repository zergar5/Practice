using Application.FEM._2D;
using Application.FEM.Core.Assembling.Boundaries.First;
using Application.FEM.Core.Expressions;
using Application.FEM.Core.Grid;
using Domain.Boundaries;
using Domain.Edges;
using Domain.Nodes;
using System.Numerics;

namespace Application.FEM.Assembling._2D.Boundaries.First;

public interface IFirstBoundaryExpressionResolver2D<out T> : IFirstBoundaryExpressionResolver<T, Edge<Node2D>> where T : INumberBase<T>;

public class FirstBoundaryExpressionResolver2D<T> : IFirstBoundaryExpressionResolver2D<T> where T : INumberBase<T>
{
    private readonly ExpressionParser2D _expressionParser;
    private readonly IGrid<Node2D, IElement2D> _grid;

    public FirstBoundaryExpressionResolver2D(ExpressionParser2D expressionParser, IGrid<Node2D, IElement2D> grid)
    {
        _expressionParser = expressionParser;
        _grid = grid;
    }

    public IFirstBoundaryValue<T>[] ResolveBoundaryValues(IBoundaryConditionExpression<Edge<Node2D>> bound)
    {
        throw new NotImplementedException();
    }
}