using Application.FEM.Core.Grid;
using Application.MathObjects.Matrices;

namespace Application.FEM.Core.Assembling.Local;

public interface ILocalStiffnessMatrixAssembler<in TElement> where TElement : IElement
{
    public IMatrix<double> AssembleStiffnessMatrix(TElement element);
}

public interface ILocalMassMatrixAssembler<in TElement> where TElement : IElement
{
    public IMatrix<double> AssembleMassMatrix(TElement element);
}

public interface ILocalMatricesAssembler<in TElement> :
    ILocalStiffnessMatrixAssembler<TElement>, ILocalMassMatrixAssembler<TElement>
    where TElement : IElement;