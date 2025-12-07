using Application.InverseProblem.Parameters;

namespace Application.InverseProblem.Assembling.Derivatives._2D.Harmonic;

public interface IHarmonicMeasurementDerivativesCalculator
{
    public void Init();
    public double[,] CalcDerivativesForParameter(Parameter parameter, double[,] measurements);
}