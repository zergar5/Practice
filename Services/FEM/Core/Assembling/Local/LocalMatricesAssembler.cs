using Application.FEM.Core.Grid;
using Application.MathObjects.Matrices;
using System.Numerics;

namespace Application.FEM.Core.Assembling.Local;

public interface ILocalStiffnessMatrixAssembler<in TElement, T>
    where TElement : IElement
    where T : INumberBase<T>

{
    public IMatrix<T> AssembleStiffnessMatrix(TElement element);
}

public interface ILocalMassMatrixAssembler<in TElement, T>
    where TElement : IElement
    where T : INumberBase<T>
{
    public IMatrix<T> AssembleMassMatrix(TElement element);
}

public interface ILocalMatricesAssembler<in TElement, T> :
    ILocalStiffnessMatrixAssembler<TElement, T>, ILocalMassMatrixAssembler<TElement, T>
    where TElement : IElement
    where T : INumberBase<T>;