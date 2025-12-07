using Application.FEM._1D.MatrixTemplates;
using Application.FEM.Assembling._1D;
using Application.MathObjects.Matrices;

namespace Application.FEM._1D.Assembling.Local;

public class LocalStiffnessMatrixAssembler1D : ILocalStiffnessMatrixAssembler1D
{
    private readonly IMatrix<int> _stiffnessMatrix;

    public LocalStiffnessMatrixAssembler1D()
    {
        _stiffnessMatrix = LagrangeMatrixTemplates.StiffnessMatrix;
    }

    public IMatrix<double> AssembleStiffnessMatrix(double elementSize)
    {
        return _stiffnessMatrix.Multiply(1d / elementSize, MatrixPool<double>.Rent(_stiffnessMatrix.RowCount));
    }
}

public class LocalMassMatrixAssembler1D : ILocalMassMatrixAssembler1D
{
    private readonly IMatrix<int> _massMatrix;

    public LocalMassMatrixAssembler1D()
    {
        _massMatrix = LagrangeMatrixTemplates.MassMatrix;
    }

    public IMatrix<double> AssembleMassMatrix(double elementSize)
    {
        return _massMatrix.Multiply(elementSize / 6, MatrixPool<double>.Rent(_massMatrix.RowCount));
    }
}