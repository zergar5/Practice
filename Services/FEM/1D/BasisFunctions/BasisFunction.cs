using System.Numerics;
using Application.FEM.Core.BasisFunctions;
using Domain.Nodes;

namespace Application.FEM._1D.BasisFunctions;

public class BasisFunction<TResult> : IBasisFunction<INode, TResult>
    where TResult : INumber<TResult>
{
    private readonly Func<double, TResult> _xFunction;

    public BasisFunction(Func<double, TResult> xFunction)
    {
        _xFunction = xFunction;
    }

    public TResult Evaluate(INode node) => _xFunction(node.X);
}