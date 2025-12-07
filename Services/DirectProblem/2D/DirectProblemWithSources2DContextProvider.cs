using Domain.Environment;
using Node2D = Domain.Nodes.Node2D;

namespace Application.DirectProblem._2D;

public class DirectProblemWithSources2DContextProvider : IDirectProblemContextProvider<DirectProblemWithSources2DContext<Node2D>>
{
    private readonly DirectProblemWithSources2DContext<Node2D> _context;

    public DirectProblemWithSources2DContextProvider()
    {
        _context = new DirectProblemWithSources2DContext<Node2D>();
    }

    public DirectProblemWithSources2DContext<Node2D> Get() => _context;
}

public class DirectProblemWithSources2DContext<TLocation> : DirectProblem2DContext
{
    public Source<TLocation>[] Sources { get; set; } = null!;
}