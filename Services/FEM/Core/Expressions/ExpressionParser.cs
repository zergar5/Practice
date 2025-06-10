using System.Collections.ObjectModel;
using System.Linq.Dynamic.Core;
using Domain.Nodes;
using System.Linq.Expressions;
using System.Numerics;

namespace Application.FEM.Core.Expressions;

public interface IExpressionParser
{
    public LambdaExpression Parse<T>(string expression) where T : INumberBase<T>;
}

public class ExpressionParser2D : IExpressionParser
{
    public LambdaExpression Parse<T>(string expression) where T : INumberBase<T>
    {
        var nodeParameter = Expression.Parameter(typeof(Node2D), "node");

        var xProperty = Expression.PropertyOrField(nodeParameter, nameof(Node2D.X));
        var yProperty = Expression.PropertyOrField(nodeParameter, nameof(Node2D.Y));

        var parameterReplacer = new ParameterExpressionReplacer(new Dictionary<string, Expression>
        {
            { "x", xProperty },
            { "y", yProperty },
            { "r", xProperty },
            { "z", yProperty }
        });

        var parsedBody = DynamicExpressionParser.ParseLambda([nodeParameter], typeof(T), expression).Body;

        return Expression.Lambda<Func<Node2D, T>>(parameterReplacer.Visit(parsedBody), nodeParameter);
    }
}