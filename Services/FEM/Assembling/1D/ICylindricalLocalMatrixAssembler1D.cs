using Application.MathObjects.Matrices;

namespace Application.FEM.Assembling._1D;

public interface ICylindricalLocalStiffnessMatrixAssembler1D
{
    public IMatrix<double> AssembleStiffnessMatrix(double elementSize, double r);
}

public interface ICylindricalLocalMassMatrixAssembler1D
{
    public IMatrix<double> AssembleMassMatrix(double elementSize, double r);
}