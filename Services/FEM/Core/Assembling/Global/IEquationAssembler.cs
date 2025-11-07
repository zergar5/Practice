using Application.FEM.Core.Assembling.Boundaries.First;
using Application.FEM.Core.Assembling.Local;
using Application.FEM.Core.Grid;
using Application.MathObjects.Equation;
using Domain.Environment;
using System.Numerics;

namespace Application.FEM.Core.Assembling.Global;

public interface IEquationAssembler<T, TNode, TElement, out TMatrix, in TBoundary>
    where T : INumberBase<T>
    where TElement : IElement
    where TBoundary : INumberBase<TBoundary>
{
    public IEquationAssembler<T, TNode, TElement, TMatrix, TBoundary> AllocateEquation(IGrid<TNode, TElement> grid);
    public IEquationAssembler<T, TNode, TElement, TMatrix, TBoundary> AddMatrixToEquationLeftPart(ILocalMatrixAssembler<TElement, T> localMatrixAssembler);
    public IEquationAssembler<T, TNode, TElement, TMatrix, TBoundary> AddVectorToEquationRightPart(ILocalVectorAssembler<TElement, T> localVectorAssembler);
    public IEquationAssembler<T, TNode, TElement, TMatrix, TBoundary> ApplyFirstCondition(IFirstBoundaryValue<TBoundary> firstBoundaryValue);
    public IEquationAssembler<T, TNode, TElement, TMatrix, TBoundary> ApplyFirstConditions(IFirstBoundaryValue<TBoundary>[] firstBoundaryValues);
    public IEquation<TMatrix, T> GetEquation();
}

public interface IProblemWithSourcesEquationAssembler<T, TNode, TElement, out TMatrix, in TBoundary> : IEquationAssembler<T, TNode, TElement, TMatrix, TBoundary>
    where T : INumberBase<T>
    where TElement : IElement
    where TBoundary : INumberBase<TBoundary>
{
    public new IProblemWithSourcesEquationAssembler<T, TNode, TElement, TMatrix, TBoundary> AllocateEquation(IGrid<TNode, TElement> grid);
    public new IProblemWithSourcesEquationAssembler<T, TNode, TElement, TMatrix, TBoundary> AddMatrixToEquationLeftPart(ILocalMatrixAssembler<TElement, T> localMatrixAssembler);
    public new IProblemWithSourcesEquationAssembler<T, TNode, TElement, TMatrix, TBoundary> AddVectorToEquationRightPart(ILocalVectorAssembler<TElement, T> localVectorAssembler);
    public new IProblemWithSourcesEquationAssembler<T, TNode, TElement, TMatrix, TBoundary> ApplyFirstCondition(IFirstBoundaryValue<TBoundary> firstBoundaryValue);
    public new IProblemWithSourcesEquationAssembler<T, TNode, TElement, TMatrix, TBoundary> ApplyFirstConditions(IFirstBoundaryValue<TBoundary>[] firstBoundaryValues);
    public IProblemWithSourcesEquationAssembler<T, TNode, TElement, TMatrix, TBoundary> AccountSource(Source<TNode> source);
    public IProblemWithSourcesEquationAssembler<T, TNode, TElement, TMatrix, TBoundary> AccountSources(Source<TNode>[] sources);
}