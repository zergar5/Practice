using DirectProblem.Core;
using DirectProblem.Core.Base;
using DirectProblem.Core.Boundary;
using DirectProblem.Core.Global;
using DirectProblem.Core.GridComponents;
using DirectProblem.FEM;
using DirectProblem.FEM.Assembling.Local;
using DirectProblem.SLAE.Preconditions;
using DirectProblem.SLAE.Solvers;
using DirectProblem.TwoDimensional;
using DirectProblem.TwoDimensional.Assembling;
using DirectProblem.TwoDimensional.Assembling.Boundary;
using DirectProblem.TwoDimensional.Assembling.Global;
using DirectProblem.TwoDimensional.Assembling.Local;
using DirectProblem.TwoDimensional.Assembling.MatrixTemplates;
using DirectProblem.TwoDimensional.Parameters;
using System.Diagnostics;

namespace DirectProblem;

public class DirectProblemSolver
{
    private static readonly MatrixPortraitBuilder MatrixPortraitBuilder = new();
    private static readonly Inserter Inserter = new();
    private static readonly LinearFunctionsProvider LinearFunctionsProvider = new();
    private static readonly GaussExcluder GaussExcluder = new();
    private static readonly LOS LOS = new(new LUPreconditioner(), new LUSparse());
    private readonly Grid<Node2D> _grid;
    private readonly MaterialFactory _materialFactory;
    private readonly LocalBasisFunctionsProvider _localBasisFunctionsProvider;
    private readonly FirstConditionValue[] _firstConditions;
    private double _omega;
    private FocusedSource _focusedSource;
    private Equation<SparseMatrix> _equation;

    public DirectProblemSolver
    (
        Grid<Node2D> grid, 
        MaterialFactory materialFactory,
        FirstConditionValue[] firstConditions
    )
    {
        _grid = grid;
        _materialFactory = materialFactory;
        _localBasisFunctionsProvider = new LocalBasisFunctionsProvider(grid, LinearFunctionsProvider);
        _firstConditions = firstConditions;
    }

    public DirectProblemSolver SetOmega(double omega)
    {
        _omega = omega;

        return this;
    }

    public DirectProblemSolver SetSource(FocusedSource focusedSource)
    {
        _focusedSource = focusedSource;

        return this;
    }

    public DirectProblemSolver AssembleSLAE()
    {
        var localAssembler =
            new LocalAssembler(
                new LocalMatrixAssembler(_grid),
                _materialFactory, _omega);

        var globalAssembler = new GlobalAssembler<Node2D>(_grid, MatrixPortraitBuilder, localAssembler, Inserter,
            GaussExcluder, _localBasisFunctionsProvider);

        _equation = globalAssembler
            .AssembleEquation(_grid)
            .ApplySources(_focusedSource)
            .ApplyFirstConditions(_firstConditions)
            .BuildEquation();

        var preconditionMatrix = globalAssembler.AllocatePreconditionMatrix();
        LOS.SetPrecondition(preconditionMatrix);

        return this;
    }

    public Vector Solve()
    {
        var solution = LOS.Solve(_equation);

        return solution;
    }
}