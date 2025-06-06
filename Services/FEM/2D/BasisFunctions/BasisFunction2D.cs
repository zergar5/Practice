using Application.FEM.Core.BasisFunctions;
using Domain.Nodes;

namespace Application.FEM._2D.BasisFunctions;

public class BasisFunction2D : IBasisFunction<Node2D>
{
    private readonly IBasisFunction<double> _xFunction;
    private readonly IBasisFunction<double> _yFunction;

    public BasisFunction2D(IBasisFunction<double> xFunction, IBasisFunction<double> yFunction)
    {
        _xFunction = xFunction;
        _yFunction = yFunction;
    }

    public double Evaluate(Node2D node) => _xFunction.Evaluate(node.X) * _yFunction.Evaluate(node.Y);
}