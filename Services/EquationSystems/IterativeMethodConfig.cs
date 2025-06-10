namespace Application.EquationSystems;

public class IterativeMethodConfig
{
    public double ResidualPrecision { get; }
    public int MaxIterations { get; }

    public IterativeMethodConfig(int maxIterations, double residualPrecision)
    {
        ResidualPrecision = residualPrecision;
        MaxIterations = maxIterations;
    }

    public IterativeMethodConfig()
    {
        ResidualPrecision = 1e-15;
        MaxIterations = 10000;
    }
}