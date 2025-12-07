using Application.FEM.Core.Grid;
using Application.MathObjects.Matrices;

namespace Application.FEM.Core.Assembling.Local;

public interface ICylindricalLocalStiffnessMatrixAssembler<in TElement> where TElement : IElement
{
    public IMatrix<double> AssembleStiffnessMatrix(TElement element, double r);
}

public interface ICylindricalLocalMassMatrixAssembler<in TElement> where TElement : IElement
{
    public IMatrix<double> AssembleMassMatrix(TElement element, double r);
}

public interface ICylindricalLocalMatricesAssembler<in TElement> :
    ICylindricalLocalStiffnessMatrixAssembler<TElement>, ICylindricalLocalMassMatrixAssembler<TElement>
    where TElement : IElement;