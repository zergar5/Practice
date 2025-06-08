using Application.MathObjects.Matrices;
using System.Numerics;

namespace Application.FEM.Assembling._1D;

public interface ICylindricalLocalStiffnessMatrixAssembler1D<T> where T : INumberBase<T>
{
    public IMatrix<T> AssembleStiffnessMatrix(double elementSize, double r);
}

public interface ICylindricalLocalMassMatrixAssembler1D<T> where T : INumberBase<T>
{
    public IMatrix<T> AssembleMassMatrix(double elementSize, double r);
}