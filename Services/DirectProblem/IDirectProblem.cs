using Application.FEM.Core.Grid;
using Domain.Boundaries;
using Domain.Environment;
using System.Numerics;
using Domain.Nodes;

namespace Application.DirectProblem;

public interface IDirectProblem<T, TNode, out TElement, in TGridParameters, TBoundaryAttachment, in TMaterial>
    where T : INumberBase<T>
    where TElement : IElement
{
    public IGrid<TNode, TElement> GenerateGrid(TGridParameters gridParameters);
    public void SetMaterials(TMaterial[] materials);
    public void SetDefinedValueBoundaryCondition(IDefinedValueBoundaryCondition<TBoundaryAttachment, T>[] definedValueBoundaryCondition);
    public ISolutionResolver<T, TNode> Solve();
}

public interface IDirectProblemWithSources<T, TNode, out TElement, in TGridParameters, TBoundaryAttachment, in TMaterial> : IDirectProblem<T, TNode, TElement, TGridParameters, TBoundaryAttachment, TMaterial>
    where T : INumberBase<T>
    where TElement : IElement
{
    public void SetSources(Source<TNode>[] sources);
}

public interface IHarmonicDirectProblem<T, TNode, out TElement, in TGridParameters, TBoundaryAttachment, in TMaterial> : IDirectProblem<T, TNode, TElement, TGridParameters, TBoundaryAttachment, TMaterial>
    where T : INumberBase<T>
    where TElement : IElement
{
    public void SetFrequency(double frequency);
}