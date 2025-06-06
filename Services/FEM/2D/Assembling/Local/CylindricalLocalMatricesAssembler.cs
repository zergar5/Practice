using Application.FEM.Core.Assembling.Local;
using Application.MathObjects.Matrices;

namespace Application.FEM._2D.Assembling.Local;

public class RotorLocalMatricesAssembler2D : ICylindricalLocalMatricesAssembler<Element2D, double>
{
    private readonly ICylindricalLocalStiffnessMatrixAssembler<Element2D, double> _cylindricalLocalStiffnessMatrixAssembler;
    private readonly ICylindricalLocalMassMatrixAssembler<Element2D, double> _cylindricalLocalMassMatrixAssembler;

    public RotorLocalMatricesAssembler2D
    (
        ICylindricalLocalStiffnessMatrixAssembler<Element2D, double> cylindricalLocalStiffnessMatrixAssembler,
        ICylindricalLocalMassMatrixAssembler<Element2D, double> cylindricalLocalMassMatrixAssembler
    )
    {
        _cylindricalLocalStiffnessMatrixAssembler = cylindricalLocalStiffnessMatrixAssembler;
        _cylindricalLocalMassMatrixAssembler = cylindricalLocalMassMatrixAssembler;
    }

    public IMatrix<double> AssembleStiffnessMatrix(Element2D element, double r) =>
        _cylindricalLocalStiffnessMatrixAssembler.AssembleStiffnessMatrix(element, r);

    public IMatrix<double> AssembleMassMatrix(Element2D element, double r) =>
        _cylindricalLocalMassMatrixAssembler.AssembleMassMatrix(element, r);
}