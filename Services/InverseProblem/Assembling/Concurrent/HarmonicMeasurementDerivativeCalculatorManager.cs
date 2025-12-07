using Application.InverseProblem.Assembling.Derivatives._2D.Harmonic;
using System.Collections.Concurrent;

namespace Application.InverseProblem.Assembling.Concurrent;

public class HarmonicMeasurementDerivativeCalculatorManager : IMeasurementCalculatorManager<IHarmonicMeasurementDerivativesCalculator>
{
    private readonly ConcurrentQueue<IHarmonicMeasurementDerivativesCalculator> _calculators;

    public HarmonicMeasurementDerivativeCalculatorManager(IHarmonicMeasurementDerivativesCalculator[] calculators)
    {
        _calculators = new ConcurrentQueue<IHarmonicMeasurementDerivativesCalculator>(calculators);
    }

    public IHarmonicMeasurementDerivativesCalculator[] GetAllFreeCalculators()
    {
        return _calculators.ToArray();
    }

    public IHarmonicMeasurementDerivativesCalculator WaitFreeCalculator()
    {
        while (true)
        {
            if (_calculators.TryDequeue(out var calculator))
            {
                return calculator;
            }
        }
    }

    public void ReleaseCalculator(IHarmonicMeasurementDerivativesCalculator calculator)
    {
        _calculators.Enqueue(calculator);
    }
}