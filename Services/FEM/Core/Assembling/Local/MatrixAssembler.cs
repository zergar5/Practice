using System.Numerics;
using Services.MathObjects.Matrices;

namespace Application.FEM.Core.Assembling.Local;

public interface IMatrixAssembler<in TElement> where TElement : IElement
{
    public IMatrix<T> AssembleStiffnessMatrix<T>(TElement element) where T : INumber<T>;
    public IMatrix<T> AssembleMassMatrix<T>(TElement element) where T : INumber<T>;
}