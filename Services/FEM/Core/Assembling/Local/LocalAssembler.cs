using Application.FEM.Core.Grid;

namespace Application.FEM.Core.Assembling.Local;

public interface ILocalMatrixAssembler<in TElement> where TElement : IElement
{
    public ILocalMatrix<double> AssembleMatrix(TElement element);
}

public interface ILocalVectorAssembler<in TElement> where TElement : IElement
{
    public ILocalVector<double> AssembleVector(TElement element);
}

public interface ILocalAssembler<in TElement> : ILocalMatrixAssembler<TElement>, ILocalVectorAssembler<TElement>
    where TElement : IElement;