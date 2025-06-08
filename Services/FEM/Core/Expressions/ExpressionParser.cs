using System.Linq.Expressions;
using System.Numerics;
using Domain.Nodes;

namespace Application.FEM.Core.Expressions;

public interface IExpressionParser
{
    public LambdaExpression Parse(string expression);
}

public class ExpressionParser2D<T> : IExpressionParser where T : INumberBase<T>
{
    public LambdaExpression Parse(string expression)
    {
        var pointParam = Expression.Parameter(typeof(Node2D), "node");

        var xProperty = Expression.PropertyOrField(pointParam, nameof(Node2D.X));
        var yProperty = Expression.PropertyOrField(pointParam, nameof(Node2D.Y));

        //var parameterReplacer = new ParameterExpressionReplacer(
        //    ("x", xProperty),
        //    ("y", yProperty)
        //);

        var parsedBody = System.Linq.Dynamic.Core.DynamicExpressionParser
            .ParseLambda([pointParam], typeof(T), expression)
            .Body;

        //var body = parameterReplacer.Visit(parsedBody);

        var parsedExpression = Expression.Lambda<Func<Node2D, double>>(parsedBody, pointParam);

        return Expression.Lambda<Func<Node2D, double, double>>(parsedBody, pointParam).;
    }

    private void Exec()
    {
        var expr = Parse("");
        var ktok = expr.Compile();
    }
}