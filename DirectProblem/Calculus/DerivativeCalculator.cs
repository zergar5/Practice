using DirectProblem.Core.GridComponents;
using DirectProblem.TwoDimensional.Assembling.Local;

namespace DirectProblem.Calculus;

public class DerivativeCalculator
{
    private const double Delta = 1.0e-3;

    public double Calculate(LocalBasisFunction localBasisFunction, Node2D point, char variableChar)
    {
        double result;
        if (variableChar == 'r')
        {
            result = localBasisFunction.Calculate(point.R + Delta, point.Z) - localBasisFunction.Calculate(point.R - Delta, point.Z);
        }
        else
        {
            result = localBasisFunction.Calculate(point.R, point.Z + Delta) - localBasisFunction.Calculate(point.R, point.Z - Delta);
        }
        return result / (2.0 * Delta);
    }
}