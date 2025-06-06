using Application.FEM.Assembling._1D;
using Application.FEM.Core.Assembling.Local;
using Application.MathObjects.Matrices;

namespace Application.FEM._2D.Assembling.Local;

// TODO все матрицы нужно иницилизировать в скопе и переиспользовать
public class RotorLocalStiffnessMatrixAssembler2D : ICylindricalLocalStiffnessMatrixAssembler<Element2D, double>
{
    private readonly ICylindricalLocalStiffnessMatrixAssembler1D<double> _cylindricalLocalRotorStiffnessMatrixAssembler1D;
    private readonly ICylindricalLocalStiffnessMatrixAssembler1D<double> _cylindricalLocalStiffnessMatrixAssembler1D;
    private readonly ILocalStiffnessMatrixAssembler1D<double> _localStiffnessMatrixAssembler1D;
    private readonly ICylindricalLocalMassMatrixAssembler1D<double> _cylindricalLocalMassMatrixAssembler1D;
    private readonly ILocalMassMatrixAssembler1D<double> _localMassMatrixAssembler1D;

    public RotorLocalStiffnessMatrixAssembler2D
    (
        ICylindricalLocalStiffnessMatrixAssembler1D<double> cylindricalLocalRotorStiffnessMatrixAssembler1D,
        ICylindricalLocalStiffnessMatrixAssembler1D<double> cylindricalLocalStiffnessMatrixAssembler1D,
        ILocalStiffnessMatrixAssembler1D<double> localStiffnessMatrixAssembler1D,
        ICylindricalLocalMassMatrixAssembler1D<double> cylindricalLocalMassMatrixAssembler1D,
        ILocalMassMatrixAssembler1D<double> localMassMatrixAssembler1D
    )
    {
        _cylindricalLocalRotorStiffnessMatrixAssembler1D = cylindricalLocalRotorStiffnessMatrixAssembler1D;
        _cylindricalLocalStiffnessMatrixAssembler1D = cylindricalLocalStiffnessMatrixAssembler1D;
        _localStiffnessMatrixAssembler1D = localStiffnessMatrixAssembler1D;
        _cylindricalLocalMassMatrixAssembler1D = cylindricalLocalMassMatrixAssembler1D;
        _localMassMatrixAssembler1D = localMassMatrixAssembler1D;
    }

    public IMatrix<double> AssembleStiffnessMatrix(Element2D element, double r)
    {
        var rotorStiffness = _cylindricalLocalRotorStiffnessMatrixAssembler1D.AssembleStiffnessMatrix(element.Length, r);

        var stiffnessR = _cylindricalLocalStiffnessMatrixAssembler1D.AssembleStiffnessMatrix(element.Length, r);
        var stiffnessZ = _localStiffnessMatrixAssembler1D.AssembleStiffnessMatrix(element.Height);

        var massR = _cylindricalLocalMassMatrixAssembler1D.AssembleMassMatrix(element.Length, r);
        var massZ = _localMassMatrixAssembler1D.AssembleMassMatrix(element.Height);

        var stiffness = new Matrix<double>(4);

        for (var i = 0; i < element.NodeIndexes.Length; i++)
        {
            for (var j = 0; j <= i; j++)
            {
                stiffness[i, j] = stiffnessR[GetMuIndex(i), GetMuIndex(j)] * massZ[GetNuIndex(i), GetNuIndex(j)] +
                                   massR[GetMuIndex(i), GetMuIndex(j)] * stiffnessZ[GetNuIndex(i), GetNuIndex(j)] +
                                   rotorStiffness[GetMuIndex(i), GetMuIndex(j)] * massZ[GetNuIndex(i), GetNuIndex(j)];
                stiffness[j, i] = stiffness[i, j];
            }
        }

        return stiffness;
    }

    private static int GetMuIndex(int i) => i % 2;
    private int GetNuIndex(int i) => i / 2;
}

public class CylindricalLocalMassMatrixAssembler2D : ICylindricalLocalStiffnessMatrixAssembler<Element2D, double>
{
    private readonly ICylindricalLocalMassMatrixAssembler1D<double> _cylindricalLocalMassMatrixAssembler1D;
    private readonly ILocalMassMatrixAssembler1D<double> _localMassMatrixAssembler1D;

    public CylindricalLocalMassMatrixAssembler2D
    (
        ICylindricalLocalMassMatrixAssembler1D<double> cylindricalLocalMassMatrixAssembler1D,
        ILocalMassMatrixAssembler1D<double> localMassMatrixAssembler1D
    )
    {
        _cylindricalLocalMassMatrixAssembler1D = cylindricalLocalMassMatrixAssembler1D;
        _localMassMatrixAssembler1D = localMassMatrixAssembler1D;
    }

    public IMatrix<double> AssembleStiffnessMatrix(Element2D element, double r)
    {
        var massR = _cylindricalLocalMassMatrixAssembler1D.AssembleMassMatrix(element.Length, r);
        var massZ = _localMassMatrixAssembler1D.AssembleMassMatrix(element.Height);

        var mass = new Matrix<double>(4);

        for (var i = 0; i < element.NodeIndexes.Length; i++)
        {
            for (var j = 0; j <= i; j++)
            {
                mass[i, j] = massR[GetMuIndex(i), GetMuIndex(j)] * massZ[GetNuIndex(i), GetNuIndex(j)];
                mass[j, i] = mass[i, j];
            }
        }

        return mass;
    }

    private static int GetMuIndex(int i) => i % 2;
    private int GetNuIndex(int i) => i / 2;
}