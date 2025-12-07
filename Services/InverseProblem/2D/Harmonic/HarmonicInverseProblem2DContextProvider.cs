namespace Application.InverseProblem._2D.Harmonic;

public class HarmonicInverseProblem2DContextProvider : IInverseProblemContextProvider<HarmonicInverseProblem2DContext>
{
    private readonly HarmonicInverseProblem2DContext _context;

    public HarmonicInverseProblem2DContextProvider()
    {
        _context = new HarmonicInverseProblem2DContext();
    }

    public HarmonicInverseProblem2DContext Get() => _context;
}

public class HarmonicInverseProblem2DContext : InverseProblem2DContext
{
    public double[] Frequencies { get; set; } = null!;
}