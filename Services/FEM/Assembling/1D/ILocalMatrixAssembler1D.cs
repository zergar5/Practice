using Application.MathObjects.Matrices;

namespace Application.FEM.Assembling._1D;

public interface ILocalStiffnessMatrixAssembler1D
{
    public IMatrix<double> AssembleStiffnessMatrix(double elementSize);
}

public interface ILocalMassMatrixAssembler1D
{
    public IMatrix<double> AssembleMassMatrix(double elementSize);
}