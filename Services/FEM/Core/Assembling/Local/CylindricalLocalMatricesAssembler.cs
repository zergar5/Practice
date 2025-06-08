using Application.FEM.Core.Grid;
using Application.MathObjects.Matrices;
using System.Numerics;

namespace Application.FEM.Core.Assembling.Local;

public interface ICylindricalLocalStiffnessMatrixAssembler<in TElement, T>
    where TElement : IElement
    where T : INumberBase<T>
{
    public IMatrix<T> AssembleStiffnessMatrix(TElement element, double r);
}

public interface ICylindricalLocalMassMatrixAssembler<in TElement, T>
    where TElement : IElement
    where T : INumberBase<T>
{
    public IMatrix<T> AssembleMassMatrix(TElement element, double r);
}

public interface ICylindricalLocalMatricesAssembler<in TElement, T> :
    ICylindricalLocalStiffnessMatrixAssembler<TElement, T>, ICylindricalLocalMassMatrixAssembler<TElement, T>
    where TElement : IElement
    where T : INumberBase<T>;