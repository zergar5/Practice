using Application.FEM.Core.BasisFunctions;
using Domain.Nodes;

namespace Application.FEM._2D.BasisFunctions;

public class BasisFunction2D : IBasisFunction<Node2D, double>
{
    private readonly IBasisFunction<double, double> _xFunction;
    private readonly IBasisFunction<double, double> _yFunction;

    public BasisFunction2D(IBasisFunction<double, double> xFunction, IBasisFunction<double, double> yFunction)
    {
        _xFunction = xFunction;
        _yFunction = yFunction;
    }

    public double Evaluate(Node2D node) => _xFunction.Evaluate(node.X) * _yFunction.Evaluate(node.Y);
}