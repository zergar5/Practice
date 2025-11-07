using Application.FEM._2D;
using Application.FEM.Core.Grid;
using Domain.Nodes;
using static Application.FEM._2D.Grid.GridBuilder2D;

namespace Application.DirectProblem._2D;

public class DirectProblem2DContextProvider : IDirectProblemContextProvider<DirectProblem2DContext>
{
    private readonly DirectProblem2DContext _context;

    public DirectProblem2DContextProvider()
    {
        _context = new DirectProblem2DContext();
    }

    public DirectProblem2DContext Get() => _context;
}

public class DirectProblem2DContext
{
    public Grid2DParameters GridParameters { get; set; } = null!;
    public IGrid<Node2D, IElement2D> Grid { get; set; } = null!;
}