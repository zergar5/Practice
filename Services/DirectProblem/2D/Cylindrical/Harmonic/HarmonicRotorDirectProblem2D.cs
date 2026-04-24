using Application.EquationSystems.Solvers.Sparse;
using Application.FEM._2D;
using Application.FEM.Core.Assembling.Boundaries.First;
using Application.FEM.Core.Assembling.Global;
using Application.FEM.Core.Assembling.Local;
using Application.FEM.Core.BasisFunctions;
using Application.FEM.Core.Grid;
using Application.MathObjects.Matrices;
using Domain.Boundaries;
using Domain.Edges;
using Domain.Enums;
using Domain.Environment;
using Domain.Materials;
using Domain.Nodes;
using System.Numerics;
using static Application.FEM._2D.Grid.GridBuilder2D;

namespace Application.DirectProblem._2D.Cylindrical.Harmonic;

public class HarmonicRotorDirectProblem2D : IHarmonicDirectProblem2DWithSources<Complex, Grid2DParameters, MaterialWithSigmaMu>
{
    private readonly IDirectProblemContextProvider<HarmonicRotorDirectProblem2DContext> _problemContextProvider;
    private readonly IGridBuilder<Node2D, IElement2D, Grid2DParameters> _gridBuilder;
    private readonly IBasisFunctionsProvider<Node2D, double, IElement2D> _basisFunctionsProvider;
    private readonly ILocalMatrixAssembler<IElement2D> _localMatrixAssembler;
    private readonly IDefinedValueFirstBoundaryResolver<Complex, Edge<Node2D>> _definedValueFirstBoundaryResolver;
    private readonly IProblemWithSourcesEquationAssembler<Node2D, IElement2D, ISparseMatrix<double>, Complex> _equationAssembler;
    private readonly ISparseSLAESolver _slaeSolver;

    private IDefinedValueBoundaryCondition<Edge<Node2D>, Complex>[] _definedValueBoundaryConditions = [];
    private Source<Node2D>[] _sources;

    public HarmonicRotorDirectProblem2D
    (
        IDirectProblemContextProvider<HarmonicRotorDirectProblem2DContext> problemContextProvider,
        IGridBuilder<Node2D, IElement2D, Grid2DParameters> gridBuilder,
        IBasisFunctionsProvider<Node2D, double, IElement2D> basisFunctionsProvider,
        ILocalMatrixAssembler<IElement2D> localMatrixAssembler,
        IDefinedValueFirstBoundaryResolver<Complex, Edge<Node2D>> definedValueFirstBoundaryResolver,
        IProblemWithSourcesEquationAssembler<Node2D, IElement2D, ISparseMatrix<double>, Complex> equationAssembler,
        ISparseSLAESolver slaeSolver
    )
    {
        _problemContextProvider = problemContextProvider;
        _gridBuilder = gridBuilder;
        _basisFunctionsProvider = basisFunctionsProvider;
        _localMatrixAssembler = localMatrixAssembler;
        _definedValueFirstBoundaryResolver = definedValueFirstBoundaryResolver;
        _equationAssembler = equationAssembler;
        _slaeSolver = slaeSolver;
    }

    public IGrid<Node2D, IElement2D> GenerateGrid(Grid2DParameters gridParameters)
    {
        var grid = _gridBuilder.Build(gridParameters);

        Console.WriteLine($"Grid built with node count {grid.Nodes.Count}");

        var context = _problemContextProvider.Get();

        context.Grid = grid;

        return grid;
    }

    public void SetGrid(IGrid<Node2D, IElement2D> grid)
    {
        var context = _problemContextProvider.Get();

        context.Grid = grid;
    }

    public void SetMaterials(MaterialWithSigmaMu[] materials)
    {
        var context = _problemContextProvider.Get();

        context.Materials = materials;
    }

    public void SetDefinedValueBoundaryCondition(IDefinedValueBoundaryCondition<Edge<Node2D>, Complex>[] definedValueBoundaryCondition)
    {
        _definedValueBoundaryConditions = definedValueBoundaryCondition;
    }

    public void SetSources(Source<Node2D>[] sources)
    {
        _sources = sources;
    }

    public void SetFrequency(double frequency)
    {
        var context = _problemContextProvider.Get();

        context.Frequency = frequency;
    }

    public ISolutionResolver<Complex, Node2D> Solve()
    {
        var context = _problemContextProvider.Get();
        var firstConditionValues = _definedValueBoundaryConditions.Where(c => c.Type == BoundaryConditionType.First).SelectMany(_definedValueFirstBoundaryResolver.ResolveBoundaryValues).ToArray();

        _equationAssembler
            .AllocateEquation(context.Grid)
            .AddMatrixToEquationLeftPart(_localMatrixAssembler);

        _equationAssembler
            .AccountSources(_sources)
            .ApplyFirstConditions(firstConditionValues);

        var equation = _equationAssembler.GetEquation();
        var solutionVector = _slaeSolver.Solve(equation);

        return new ComplexSolutionResolver2D(context.Grid, solutionVector, _basisFunctionsProvider);
    }
}