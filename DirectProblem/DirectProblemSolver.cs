using DirectProblem.Core;
using DirectProblem.Core.Base;
using DirectProblem.Core.Boundary;
using DirectProblem.Core.Global;
using DirectProblem.Core.GridComponents;
using DirectProblem.SLAE.Preconditions;
using DirectProblem.SLAE.Solvers;
using DirectProblem.TwoDimensional.Assembling;
using DirectProblem.TwoDimensional.Assembling.Boundary;
using DirectProblem.TwoDimensional.Assembling.Global;
using DirectProblem.TwoDimensional.Assembling.Local;

namespace DirectProblem;

public class DirectProblemSolver
{
    private static readonly MatrixPortraitBuilder MatrixPortraitBuilder = new();
    private static readonly Inserter Inserter = new();
    private static readonly GaussExcluder GaussExcluder = new();
    private static readonly LOS LOS = new(new LUPreconditioner(), new LUSparse());

    private readonly LocalBasisFunctionsProvider _localBasisFunctionsProvider;
    private readonly LocalMatrixAssembler _localMatrixAssembler;
    private readonly LocalAssembler _localAssembler;
    private readonly FirstBoundaryProvider _firstBoundaryProvider;
    private readonly GlobalAssembler<Node2D> _globalAssembler;

    private Grid<Node2D> _grid;
    private Source _source;
    private FirstConditionValue[] _firstConditions;

    private Equation<SparseMatrix> _equation;

    public DirectProblemSolver(Grid<Node2D> grid, Material[] materials)
    {
        _grid = grid;

        _localBasisFunctionsProvider = new LocalBasisFunctionsProvider(grid);
        _localMatrixAssembler = new LocalMatrixAssembler(grid);
        _localAssembler = new LocalAssembler(_localMatrixAssembler, materials);
        _firstBoundaryProvider = new FirstBoundaryProvider(grid);
        _firstConditions = _firstBoundaryProvider.GetConditions(
            grid.Nodes.RLength - 1, grid.Nodes.ZLength - 1);
        _globalAssembler = new GlobalAssembler<Node2D>(grid, MatrixPortraitBuilder,
            _localAssembler, Inserter, GaussExcluder);
    }

    public DirectProblemSolver SetGrid(Grid<Node2D> grid)
    {
        _grid = grid;
        _localBasisFunctionsProvider.SetGrid(grid);
        _localMatrixAssembler.SetGrid(grid);
        _firstBoundaryProvider.SetGrid(grid);
        _firstConditions = _firstBoundaryProvider.GetConditions(
                grid.Nodes.RLength - 1, grid.Nodes.ZLength - 1);

        return this;
    }

    public DirectProblemSolver SetMaterials(Material[] materials)
    {
        _localAssembler.SetMaterials(materials);

        return this;
    }

    public DirectProblemSolver SetFrequency(double frequency)
    {
        _localAssembler.SetFrequency(frequency);

        return this;
    }

    public DirectProblemSolver SetSource(Source focusedSource)
    {
        _source = focusedSource;

        return this;
    }

    public DirectProblemSolver AssembleSLAE()
    {
        _equation = _globalAssembler
            .AssembleEquation(_grid)
            .ApplySources(_source)
            .ApplyFirstConditions(_firstConditions)
            .BuildEquation();

        return this;
    }

    public Vector Solve()
    {
        var preconditionMatrix = _globalAssembler.AllocatePreconditionMatrix();
        LOS.SetPrecondition(preconditionMatrix);

        var solution = LOS.Solve(_equation);

        return solution;
    }
}