using Application.FEM.Core.Grid;
using System.Numerics;

namespace Application.FEM.Core.Assembling.Local;

public interface ILocalMatrixAssembler<in TElement, out T>
    where TElement : IElement
    where T : INumberBase<T>
{
    public ILocalMatrix<T> AssembleMatrix(TElement element);
}

public interface ILocalVectorAssembler<in TElement, out T>
    where TElement : IElement
    where T : INumberBase<T>
{
    public ILocalVector<T> AssembleVector(TElement element);
}

public interface ILocalAssembler<in TElement, out T> : ILocalMatrixAssembler<TElement, T>, ILocalVectorAssembler<TElement, T>
    where TElement : IElement
    where T : INumberBase<T>;