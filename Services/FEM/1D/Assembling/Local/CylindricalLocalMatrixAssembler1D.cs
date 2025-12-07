using Application.FEM._1D.MatrixTemplates;
using Application.FEM.Assembling._1D;
using Application.MathObjects.Matrices;

namespace Application.FEM._1D.Assembling.Local;

public class CylindricalLocalStiffnessMatrixAssembler1D : ICylindricalLocalStiffnessMatrixAssembler1D
{
    private readonly IMatrix<int> _stiffnessMatrix;

    public CylindricalLocalStiffnessMatrixAssembler1D()
    {
        _stiffnessMatrix = LagrangeMatrixTemplates.StiffnessMatrix;
    }

    public IMatrix<double> AssembleStiffnessMatrix(double elementSize, double r)
    {
        return _stiffnessMatrix.Multiply((2 * r + elementSize) / (2 * elementSize), MatrixPool<double>.Rent(_stiffnessMatrix.RowCount));
    }
}

public class CylindricalLocalMassMatrixAssembler1D : ICylindricalLocalMassMatrixAssembler1D
{
    private readonly IMatrix<int> _massMatrix;
    private readonly IMatrix<int> _additionMassMatrix;

    public CylindricalLocalMassMatrixAssembler1D()
    {
        _massMatrix = LagrangeMatrixTemplates.MassMatrix;
        _additionMassMatrix = CylindricalMatrixTemplates.AdditionMassMatrix;
    }

    public IMatrix<double> AssembleMassMatrix(double elementSize, double r)
    {
        var scaledMassMatrix = _massMatrix.Multiply(elementSize * r / 6d, MatrixPool<double>.Rent(_massMatrix.RowCount));
        var scaledAdditionMassMatrix = _additionMassMatrix.Multiply(Math.Pow(elementSize, 2) / 12d, MatrixPool<double>.Rent(_additionMassMatrix.RowCount));

        scaledMassMatrix.Sum(scaledAdditionMassMatrix, scaledMassMatrix);
        MatrixPool<double>.Return(scaledAdditionMassMatrix);

        return scaledMassMatrix;
    }
}

public class RotorLocalMatrixAssembler1D : ICylindricalLocalStiffnessMatrixAssembler1D
{
    private readonly IMatrix<int> _stiffnessMatrix;
    private readonly IMatrix<int> _rotorAdditionMassMatrix;

    public RotorLocalMatrixAssembler1D()
    {
        _stiffnessMatrix = LagrangeMatrixTemplates.StiffnessMatrix;
        _rotorAdditionMassMatrix = CylindricalMatrixTemplates.RotorAdditionMassMatrix;
    }

    public IMatrix<double> AssembleStiffnessMatrix(double elementSize, double r)
    {
        var d = r / elementSize;
        var rotorStiffnessMatrix = MatrixPool<double>.Rent(_stiffnessMatrix.RowCount);

        rotorStiffnessMatrix[0, 0] = Math.Pow(1 + d, 2);
        rotorStiffnessMatrix[0, 1] = -d * (1 + d);
        rotorStiffnessMatrix[1, 0] = rotorStiffnessMatrix[0, 1];
        rotorStiffnessMatrix[1, 1] = Math.Pow(d, 2);

        rotorStiffnessMatrix.Multiply(Math.Log(1 + 1 / d), rotorStiffnessMatrix);

        var scaledStiffnessMatrix = _stiffnessMatrix.Multiply(-d, MatrixPool<double>.Rent(_stiffnessMatrix.RowCount));
        var scaledRotorAdditionMassMatrix = _rotorAdditionMassMatrix.Multiply(0.5, MatrixPool<double>.Rent(_stiffnessMatrix.RowCount));

        rotorStiffnessMatrix.Sum(scaledStiffnessMatrix, rotorStiffnessMatrix);
        rotorStiffnessMatrix.Sum(scaledRotorAdditionMassMatrix, rotorStiffnessMatrix);

        MatrixPool<double>.Return(scaledStiffnessMatrix);
        MatrixPool<double>.Return(scaledRotorAdditionMassMatrix);

        return rotorStiffnessMatrix;
    }
}