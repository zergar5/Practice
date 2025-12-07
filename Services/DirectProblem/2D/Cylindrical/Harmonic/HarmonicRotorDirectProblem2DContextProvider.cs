using Domain.Materials;
using Domain.Nodes;

namespace Application.DirectProblem._2D.Cylindrical.Harmonic;

public class HarmonicRotorDirectProblem2DContextProvider : IDirectProblemContextProvider<HarmonicRotorDirectProblem2DContext>
{
    private readonly HarmonicRotorDirectProblem2DContext _context;

    public HarmonicRotorDirectProblem2DContextProvider()
    {
        _context = new HarmonicRotorDirectProblem2DContext();
    }

    public HarmonicRotorDirectProblem2DContext Get() => _context;
}

public class HarmonicRotorDirectProblem2DContext : DirectProblemWithSources2DContext<Node2D>
{
    public MaterialWithSigmaMu[] Materials { get; set; } = null!;
    public double Frequency { get; set; }
}