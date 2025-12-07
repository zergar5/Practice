using Application.FEM.Core.Grid;

namespace Application.DirectProblem;

public interface IDirectProblemFactory<TNode, TElement, TBoundaryAttachment> where TElement : IElement
{
    //public IDirectProblem<T, TNode, TElement, TGridParameters, TBoundaryAttachment> Create<T, TGridParameters>(DirectProblemConfiguration configuration) where T : INumberBase<T>;
}