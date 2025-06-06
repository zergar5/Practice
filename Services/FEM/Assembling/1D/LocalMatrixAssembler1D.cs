using Application.MathObjects.Matrices;
using System.Numerics;

namespace Application.FEM.Assembling._1D;

public interface ILocalStiffnessMatrixAssembler1D<T> where T : INumber<T>
{
    public IMatrix<T> AssembleStiffnessMatrix(double elementSize);
}

public interface ILocalMassMatrixAssembler1D<T> where T : INumber<T>
{
    public IMatrix<T> AssembleMassMatrix(double elementSize);
}