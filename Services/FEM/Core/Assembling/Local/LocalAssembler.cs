using System.Numerics;

namespace Application.FEM.Core.Assembling.Local;

public interface ILocalAssembler
{
    public ILocalMatrix<T> AssembleMatrix<T>(IElement element) where T : INumber<T>;
    public ILocalVector<T> AssembleVector<T>(IElement element) where T : INumber<T>;
}