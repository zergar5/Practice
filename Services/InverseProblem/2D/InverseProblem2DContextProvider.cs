using Application.FEM._2D;
using Application.FEM.Core.Grid;
using Domain.Boundaries;
using Domain.Edges;
using Domain.Environment;
using Domain.Materials;
using Domain.Nodes;
using System.Numerics;
using static Application.FEM._2D.Grid.GridBuilder2D;

namespace Application.InverseProblem._2D;

public class InverseProblem2DContextProvider : IInverseProblemContextProvider<InverseProblem2DContext>
{
    private readonly InverseProblem2DContext _context;

    public InverseProblem2DContextProvider()
    {
        _context = new InverseProblem2DContext();
    }

    public InverseProblem2DContext Get() => _context;
}

public class InverseProblem2DContext
{
    public Grid2DParameters GridParameters { get; set; } = null!;
    public IGrid<Node2D, IElement2D> Grid { get; set; } = null!;
    public MaterialWithSigmaMu[] Materials { get; set; } = null!;
    public IDefinedValueBoundaryCondition<Edge<Node2D>, Complex>[] DefinedValueBoundaryConditions { get; set; } = null!;
    public Source<Node2D>[] Sources { get; set; } = null!;
    public ReceiverLine<Node2D>[] ReceiverLines { get; set; } = null!;
}