using Application.FEM._1D.MatrixTemplates;
using Application.FEM.Assembling._1D;
using Application.MathObjects.Matrices;

namespace Application.FEM._1D.Assembling.Local;

// TODO все матрицы нужно иницилизировать в скопе и переиспользовать
public class CylindricalLocalStiffnessMatrixAssembler1D : ICylindricalLocalStiffnessMatrixAssembler1D<double>
{
    public IMatrix<double> AssembleStiffnessMatrix(double elementSize, double r)
    {
        var stiffnessTemplate = LagrangeMatrixTemplates.StiffnessMatrix;
        return stiffnessTemplate.Multiply<int, double, double>((2 * r + elementSize) / (2 * elementSize));
    }
}

public class CylindricalLocalMassMatrixAssembler1D : ICylindricalLocalMassMatrixAssembler1D<double>
{
    public IMatrix<double> AssembleMassMatrix(double elementSize, double r)
    {
        var additionMassMatrix = CylindricalMatrixTemplates.AdditionMassMatrix;
        var massTemplate = LagrangeMatrixTemplates.MassMatrix;

        var scaledMassMatrix = massTemplate.Multiply<int, double, double>(elementSize * r / 6d);
        var scaledAdditionMassMatrix = additionMassMatrix.Multiply<int, double, double>(Math.Pow(elementSize, 2) / 12d);

        scaledMassMatrix.Sum(scaledAdditionMassMatrix, scaledMassMatrix);

        return scaledMassMatrix;
    }
}

public class RotorLocalMatrixAssembler : ICylindricalLocalStiffnessMatrixAssembler1D<double>
{
    public IMatrix<double> AssembleStiffnessMatrix(double elementSize, double r)
    {
        var stiffnessTemplateMatrix = LagrangeMatrixTemplates.StiffnessMatrix;
        var rotorAdditionMassMatrix = CylindricalMatrixTemplates.RotorAdditionMassMatrix;

        var d = r / elementSize;
        var rotorStiffnessMatrix = new Matrix<double>(2);

        rotorStiffnessMatrix[0, 0] = Math.Pow(1 + d, 2);
        rotorStiffnessMatrix[0, 1] = -d * (1 + d);
        rotorStiffnessMatrix[1, 0] = rotorStiffnessMatrix[0, 1];
        rotorStiffnessMatrix[1, 1] = Math.Pow(d, 2);

        rotorStiffnessMatrix.Multiply(Math.Log(1 + 1 / d), rotorStiffnessMatrix);

        rotorStiffnessMatrix.Sum(stiffnessTemplateMatrix.Multiply<int, double, double>(-d), rotorStiffnessMatrix);
        rotorStiffnessMatrix.Sum(rotorAdditionMassMatrix.Multiply<int, double, double>(0.5), rotorStiffnessMatrix);

        return rotorStiffnessMatrix;
    }
}