using Application.FEM._2D;
using Application.FEM.Core.Assembling.Boundaries.First;
using Application.FEM.Core.Assembling.Global;
using Application.FEM.Core.Grid;
using Application.MathObjects.Matrices;
using Domain.Boundaries;
using Domain.Edges;
using Domain.Enums;
using Domain.Nodes;
using System.Numerics;
using Application.EquationSystems.Solvers.Sparse;
using Application.FEM._2D.Assembling.Boundaries;
using Application.FEM._2D.BasisFunctions;
using Application.FEM.Assembling._2D.Boundaries;
using Application.FEM.Assembling._2D.Boundaries.First;
using Application.FEM.Core.Assembling.Local;
using Domain.Environment;
using Domain.Materials;
using static Application.FEM._2D.Grid.GridBuilder2D;

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