using Application.FEM.Core.BasisFunctions;

namespace Application.FEM._1D.BasisFunctions;

public class BasisFunction : IBasisFunction<double>
{
    private readonly Func<double, double> _xFunction;

    public BasisFunction(Func<double, double> xFunction)
    {
        _xFunction = xFunction;
    }

    public double Evaluate(double node) => _xFunction(node);
}