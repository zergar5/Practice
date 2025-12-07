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

public class EquationAssembler2D<TBoundary> : IEquationAssembler<Node2D, IElement2D, ISparseMatrix<double>, TBoundary>
    where TBoundary : INumberBase<TBoundary>
{
    private readonly IMatrixPortraitBuilder<Node2D, IElement2D> _matrixPortraitBuilder;
    private readonly ISparseInserter _inserter;
    private readonly IFirstBoundaryApplier<ISparseMatrix<double>, TBoundary> _firstBoundaryApplier;

    protected IGrid<Node2D, IElement2D> Grid = null!;
    protected IEquation<ISparseMatrix<double>, double> Equation = null!;

    public EquationAssembler2D
    (
        IMatrixPortraitBuilder<Node2D, IElement2D> matrixPortraitBuilder,
        ISparseInserter inserter,
        IFirstBoundaryApplier<ISparseMatrix<double>, TBoundary> firstBoundaryApplier
    )
    {
        _matrixPortraitBuilder = matrixPortraitBuilder;
        _inserter = inserter;
        _firstBoundaryApplier = firstBoundaryApplier;
    }

    public IEquationAssembler<Node2D, IElement2D, ISparseMatrix<double>, TBoundary> AllocateEquation(IGrid<Node2D, IElement2D> grid)
    {
        Grid = grid;

        var matrix = _matrixPortraitBuilder.Build(grid);

        Equation = new Equation<ISparseMatrix<double>, double>(matrix, new MathObjects.Vectors.Vector<double>(matrix.ColumnCount), new MathObjects.Vectors.Vector<double>(matrix.ColumnCount));

        return this;
    }

    public IEquationAssembler<Node2D, IElement2D, ISparseMatrix<double>, TBoundary> AddMatrixToEquationLeftPart(ILocalMatrixAssembler<IElement2D> localMatrixAssembler)
    {
        foreach (var element in Grid.Elements)
        {
            using var localMatrix = localMatrixAssembler.AssembleMatrix(element);

            _inserter.InsertMatrix(Equation.Matrix, localMatrix);
        }

        return this;
    }

    public IEquationAssembler<Node2D, IElement2D, ISparseMatrix<double>, TBoundary> AddVectorToEquationRightPart(ILocalVectorAssembler<IElement2D> localVectorAssembler)
    {
        foreach (var element in Grid.Elements)
        {
            _inserter.InsertVector(Equation.RightPart, localVectorAssembler.AssembleVector(element));
        }

        return this;
    }

    public IEquationAssembler<Node2D, IElement2D, ISparseMatrix<double>, TBoundary> ApplyFirstCondition(IFirstBoundaryValue<TBoundary> firstBoundaryValue)
    {
        _firstBoundaryApplier.Exclude(Equation, firstBoundaryValue);

        return this;
    }

    public IEquationAssembler<Node2D, IElement2D, ISparseMatrix<double>, TBoundary> ApplyFirstConditions(IFirstBoundaryValue<TBoundary>[] firstBoundaryValues)
    {
        foreach (var firstBoundaryValue in firstBoundaryValues)
        {
            ApplyFirstCondition(firstBoundaryValue);
        }

        return this;
    }

    public IEquation<ISparseMatrix<double>, double> GetEquation() => Equation;
}

public class ProblemWithSourcesEquationAssembler2D<TBoundary> : EquationAssembler2D<TBoundary>, IProblemWithSourcesEquationAssembler<Node2D, IElement2D, ISparseMatrix<double>, TBoundary>
    where TBoundary : INumberBase<TBoundary>
{
    private readonly ISourceApplier2D<ISparseMatrix<double>> _sourceApplier;

    public ProblemWithSourcesEquationAssembler2D
    (
        IMatrixPortraitBuilder<Node2D, IElement2D> matrixPortraitBuilder,
        ISparseInserter inserter,
        IFirstBoundaryApplier<ISparseMatrix<double>, TBoundary> firstBoundaryApplier,
        ISourceApplier2D<ISparseMatrix<double>> sourceApplier
    ) : base(matrixPortraitBuilder, inserter, firstBoundaryApplier)
    {
        _sourceApplier = sourceApplier;
    }

    public IProblemWithSourcesEquationAssembler<Node2D, IElement2D, ISparseMatrix<double>, TBoundary> AccountSource(Source<Node2D> source)
    {
        _sourceApplier.Apply(Equation, source);

        return this;
    }

    public IProblemWithSourcesEquationAssembler<Node2D, IElement2D, ISparseMatrix<double>, TBoundary> AccountSources(Source<Node2D>[] sources)
    {
        foreach (var source in sources)
        {
            AccountSource(source);
        }

        return this;
    }
}