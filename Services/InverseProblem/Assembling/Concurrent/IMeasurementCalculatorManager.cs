namespace Application.InverseProblem.Assembling.Concurrent;

public interface IMeasurementCalculatorManager<TMeasurementDerivativesCalculator>
{
    public TMeasurementDerivativesCalculator[] GetAllFreeCalculators();
    public TMeasurementDerivativesCalculator WaitFreeCalculator();
    public void ReleaseCalculator(TMeasurementDerivativesCalculator derivativeCalculator);
}