using System.Buffers;
using Application.FEM._2D.Assembling.Global.Sources;
using Application.FEM.Core.Assembling;
using Application.FEM.Core.Assembling.Boundaries.First;
using Application.FEM.Core.Assembling.Global;
using Application.FEM.Core.Assembling.Inserters;
using Application.FEM.Core.Assembling.Local;
using Application.FEM.Core.Grid;
using Application.MathObjects.Equation;
using Application.MathObjects.Matrices;
using Domain.Environment;
using Domain.Nodes;
using System.Numerics;

namespace Application.FEM._2D.Assembling.Global;

public class EquationAssembler2D<T, TBoundary> : IEquationAssembler<T, Node2D, IElement2D, ISparseMatrix<T>, TBoundary>
    where T : INumberBase<T>
    where TBoundary : INumberBase<TBoundary>
{
    private readonly IMatrixPortraitBuilder<Node2D, IElement2D> _matrixPortraitBuilder;
    private readonly ISparseInserter<T> _inserter;
    private readonly IFirstBoundaryApplier<ISparseMatrix<T>, T, TBoundary> _firstBoundaryApplier;

    protected IGrid<Node2D, IElement2D> Grid = null!;
    protected IEquation<ISparseMatrix<T>, T> Equation = null!;

    public EquationAssembler2D
    (
        IMatrixPortraitBuilder<Node2D, IElement2D> matrixPortraitBuilder,
        ISparseInserter<T> inserter,
        IFirstBoundaryApplier<ISparseMatrix<T>, T, TBoundary> firstBoundaryApplier
    )
    {
        _matrixPortraitBuilder = matrixPortraitBuilder;
        _inserter = inserter;
        _firstBoundaryApplier = firstBoundaryApplier;
    }

    public IEquationAssembler<T, Node2D, IElement2D, ISparseMatrix<T>, TBoundary> AllocateEquation(IGrid<Node2D, IElement2D> grid)
    {
        Grid = grid;

        var matrix = _matrixPortraitBuilder.Build<T>(grid);

        Equation = new Equation<ISparseMatrix<T>, T>(matrix, new MathObjects.Vectors.Vector<T>(matrix.ColumnCount), new MathObjects.Vectors.Vector<T>(matrix.ColumnCount));

        return this;
    }

    public IEquationAssembler<T, Node2D, IElement2D, ISparseMatrix<T>, TBoundary> AddMatrixToEquationLeftPart(ILocalMatrixAssembler<IElement2D, T> localMatrixAssembler)
    {
        foreach (var element in Grid.Elements)
        {
            using var localMatrix = localMatrixAssembler.AssembleMatrix(element);

            _inserter.InsertMatrix(Equation.Matrix, localMatrix);
        }

        return this;
    }

    public IEquationAssembler<T, Node2D, IElement2D, ISparseMatrix<T>, TBoundary> AddVectorToEquationRightPart(ILocalVectorAssembler<IElement2D, T> localVectorAssembler)
    {
        foreach (var element in Grid.Elements)
        {
            _inserter.InsertVector(Equation.RightPart, localVectorAssembler.AssembleVector(element));
        }

        return this;
    }

    public IEquationAssembler<T, Node2D, IElement2D, ISparseMatrix<T>, TBoundary> ApplyFirstCondition(IFirstBoundaryValue<TBoundary> firstBoundaryValue)
    {
        _firstBoundaryApplier.Exclude(Equation, firstBoundaryValue);

        return this;
    }

    public IEquationAssembler<T, Node2D, IElement2D, ISparseMatrix<T>, TBoundary> ApplyFirstConditions(IFirstBoundaryValue<TBoundary>[] firstBoundaryValues)
    {
        foreach (var firstBoundaryValue in firstBoundaryValues)
        {
            ApplyFirstCondition(firstBoundaryValue);
        }

        return this;
    }

    public IEquation<ISparseMatrix<T>, T> GetEquation() => Equation;
}

public class ProblemWithSourcesEquationAssembler2D<T, TBoundary> : EquationAssembler2D<T, TBoundary>, IProblemWithSourcesEquationAssembler<T, Node2D, IElement2D, ISparseMatrix<T>, TBoundary>
    where T : INumberBase<T>
    where TBoundary : INumberBase<TBoundary>
{
    private readonly ISourceApplier2D<ISparseMatrix<T>, T> _sourceApplier;

    public ProblemWithSourcesEquationAssembler2D
    (
        IMatrixPortraitBuilder<Node2D, IElement2D> matrixPortraitBuilder,
        ISparseInserter<T> inserter,
        IFirstBoundaryApplier<ISparseMatrix<T>, T, TBoundary> firstBoundaryApplier,
        ISourceApplier2D<ISparseMatrix<T>, T> sourceApplier
    ) : base(matrixPortraitBuilder, inserter, firstBoundaryApplier)
    {
        _sourceApplier = sourceApplier;
    }

    public new IProblemWithSourcesEquationAssembler<T, Node2D, IElement2D, ISparseMatrix<T>, TBoundary> AllocateEquation(IGrid<Node2D, IElement2D> grid)
    {
        base.AllocateEquation(grid);

        return this;
    }

    public new IProblemWithSourcesEquationAssembler<T, Node2D, IElement2D, ISparseMatrix<T>, TBoundary> AddMatrixToEquationLeftPart(ILocalMatrixAssembler<IElement2D, T> localMatrixAssembler)
    {
        base.AddMatrixToEquationLeftPart(localMatrixAssembler);

        return this;
    }

    public new IProblemWithSourcesEquationAssembler<T, Node2D, IElement2D, ISparseMatrix<T>, TBoundary> AddVectorToEquationRightPart(ILocalVectorAssembler<IElement2D, T> localVectorAssembler)
    {
        base.AddVectorToEquationRightPart(localVectorAssembler);

        return this;
    }

    public new IProblemWithSourcesEquationAssembler<T, Node2D, IElement2D, ISparseMatrix<T>, TBoundary> ApplyFirstCondition(IFirstBoundaryValue<TBoundary> firstBoundaryValue)
    {
        base.ApplyFirstCondition(firstBoundaryValue);

        return this;
    }

    public new IProblemWithSourcesEquationAssembler<T, Node2D, IElement2D, ISparseMatrix<T>, TBoundary> ApplyFirstConditions(IFirstBoundaryValue<TBoundary>[] firstBoundaryValues)
    {
        base.ApplyFirstConditions(firstBoundaryValues);

        return this;
    }

    public IProblemWithSourcesEquationAssembler<T, Node2D, IElement2D, ISparseMatrix<T>, TBoundary> AccountSource(Source<Node2D> source)
    {
        _sourceApplier.Apply(Equation, source);

        return this;
    }

    public IProblemWithSourcesEquationAssembler<T, Node2D, IElement2D, ISparseMatrix<T>, TBoundary> AccountSources(Source<Node2D>[] sources)
    {
        foreach (var source in sources)
        {
            AccountSource(source);
        }

        return this;
    }
}