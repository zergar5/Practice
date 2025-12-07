using Application.DirectProblem._2D;
using Application.InverseProblem.Assembling.Derivatives._2D.Harmonic;
using Domain.Materials;
using System.Collections.Concurrent;
using System.Numerics;
using static Application.FEM._2D.Grid.GridBuilder2D;

namespace Application.InverseProblem.Assembling.Concurrent;

public class HarmonicMeasurementCalculatorManager : IMeasurementCalculatorManager<IHarmonicDirectProblem2DWithSources<Complex, Grid2DParameters, MaterialWithSigmaMu>>
{
    private readonly ConcurrentQueue<IHarmonicDirectProblem2DWithSources<Complex, Grid2DParameters, MaterialWithSigmaMu>> _calculators;

    public HarmonicMeasurementCalculatorManager(IHarmonicDirectProblem2DWithSources<Complex, Grid2DParameters, MaterialWithSigmaMu>[] calculators)
    {
        _calculators = new ConcurrentQueue<IHarmonicDirectProblem2DWithSources<Complex, Grid2DParameters, MaterialWithSigmaMu>>(calculators);
    }

    public IHarmonicDirectProblem2DWithSources<Complex, Grid2DParameters, MaterialWithSigmaMu>[] GetAllFreeCalculators()
    {
        return _calculators.ToArray();
    }

    public IHarmonicDirectProblem2DWithSources<Complex, Grid2DParameters, MaterialWithSigmaMu> WaitFreeCalculator()
    {
        while (true)
        {
            if (_calculators.TryDequeue(out var calculator))
            {
                return calculator;
            }
        }
    }

    public void ReleaseCalculator(IHarmonicDirectProblem2DWithSources<Complex, Grid2DParameters, MaterialWithSigmaMu> calculator)
    {
        _calculators.Enqueue(calculator);
    }
}