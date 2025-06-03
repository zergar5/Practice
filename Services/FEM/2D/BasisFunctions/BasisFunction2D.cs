using System.Numerics;
using Application.FEM.Core.BasisFunctions;
using Domain.Nodes;

namespace Application.FEM._2D.BasisFunctions;

public class BasisFunction2D<TResult> : IBasisFunction<INode2D, TResult> where TResult : INumber<TResult>
{
    private readonly Func<double, TResult> _xFunction;
    private readonly Func<double, TResult> _yFunction;

    public BasisFunction2D(Func<double, TResult> xFunction, Func<double, TResult> yFunction)
    {
        _xFunction = xFunction;
        _yFunction = yFunction;
    }

    public TResult Evaluate(INode2D node) => _xFunction(node.X) * _yFunction(node.Y);
}