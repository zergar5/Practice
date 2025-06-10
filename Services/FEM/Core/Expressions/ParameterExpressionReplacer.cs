using System.Linq.Expressions;

namespace Application.FEM.Core.Expressions;

public class ParameterExpressionReplacer : ExpressionVisitor
{
    private readonly IReadOnlyDictionary<string, Expression> _replacements;

    public ParameterExpressionReplacer(IReadOnlyDictionary<string, Expression> replacements)
    {
        _replacements = replacements;
    }

    protected override Expression VisitParameter(ParameterExpression node)
    {
        foreach (var replacement in _replacements)
        {
            if (node.Name == replacement.Key)
            {
                return replacement.Value;
            }
        }

        return base.VisitParameter(node);
    }
}