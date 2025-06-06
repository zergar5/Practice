using Application.FEM.Core.Grid;
using System.Numerics;

namespace Application.FEM.Core.Assembling.Local;

public interface ILocalMatrixAssembler<in TElement, T>
    where TElement : IElement
    where T : INumber<T>
{
    public ILocalMatrix<T> AssembleMatrix(TElement element);
}

public interface ILocalVectorAssembler<in TElement, T>
    where TElement : IElement
    where T : INumber<T>
{
    public ILocalVector<T> AssembleVector(TElement element);
}

public interface ILocalAssembler<in TElement, T> : ILocalMatrixAssembler<TElement, T>, ILocalVectorAssembler<TElement, T>
    where TElement : IElement
    where T : INumber<T>;