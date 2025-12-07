namespace Application.InverseProblem;

public class MinimizationMethodConfig
{
    public double MinimizePrecision { get; }
    public int MaxIterations { get; }

    public MinimizationMethodConfig(int maxIterations, double minimizePrecision)
    {
        MinimizePrecision = minimizePrecision;
        MaxIterations = maxIterations;
    }

    public MinimizationMethodConfig()
    {
        MinimizePrecision = 1e-6;
        MaxIterations = 10000;
    }
}