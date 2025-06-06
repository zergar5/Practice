using Application.FEM._1D.MatrixTemplates;
using Application.FEM.Assembling._1D;
using Application.MathObjects.Matrices;

namespace Application.FEM._1D.Assembling.Local;

// TODO все матрицы нужно иницилизировать в скопе и переиспользовать
public class LocalStiffnessMatrixAssembler : ILocalStiffnessMatrixAssembler1D<double>
{
    public IMatrix<double> AssembleStiffnessMatrix(double elementSize)
    {
        var stiffnessTemplate = LagrangeMatrixTemplates.StiffnessMatrix;
        return stiffnessTemplate.Multiply<int, double, double>(1d / elementSize);
    }
}

public class LocalMassMatrixAssembler : ILocalMassMatrixAssembler1D<double>
{
    public IMatrix<double> AssembleMassMatrix(double elementSize)
    {
        var massTemplate = LagrangeMatrixTemplates.MassMatrix;
        return massTemplate.Multiply<int, double, double>(elementSize / 6);
    }
}