using Application.FEM.Core.Assembling.Boundaries.First;
using Application.FEM.Core.Assembling.Local;
using Application.FEM.Core.Grid;
using Application.MathObjects.Equation;
using Domain.Environment;
using System.Numerics;

namespace Application.FEM.Core.Assembling.Global;

public interface IEquationAssembler<in TNode, TElement, out TMatrix, in TBoundary>
    where TElement : IElement
    where TBoundary : INumberBase<TBoundary>
{
    public IEquationAssembler<TNode, TElement, TMatrix, TBoundary> AllocateEquation(IGrid<TNode, TElement> grid);
    public IEquationAssembler<TNode, TElement, TMatrix, TBoundary> AddMatrixToEquationLeftPart(ILocalMatrixAssembler<TElement> localMatrixAssembler);
    public IEquationAssembler<TNode, TElement, TMatrix, TBoundary> AddVectorToEquationRightPart(ILocalVectorAssembler<TElement> localVectorAssembler);
    public IEquationAssembler<TNode, TElement, TMatrix, TBoundary> ApplyFirstCondition(IFirstBoundaryValue<TBoundary> firstBoundaryValue);
    public IEquationAssembler<TNode, TElement, TMatrix, TBoundary> ApplyFirstConditions(IFirstBoundaryValue<TBoundary>[] firstBoundaryValues);
    public IEquation<TMatrix, double> GetEquation();
}

public interface IProblemWithSourcesEquationAssembler<TNode, TElement, out TMatrix, in TBoundary> : IEquationAssembler<TNode, TElement, TMatrix, TBoundary>
    where TElement : IElement
    where TBoundary : INumberBase<TBoundary>
{
    public IProblemWithSourcesEquationAssembler<TNode, TElement, TMatrix, TBoundary> AccountSource(Source<TNode> source);
    public IProblemWithSourcesEquationAssembler<TNode, TElement, TMatrix, TBoundary> AccountSources(Source<TNode>[] sources);
}