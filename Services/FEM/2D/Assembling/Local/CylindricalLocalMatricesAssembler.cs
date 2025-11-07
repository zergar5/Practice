using Application.FEM.Core.Assembling.Local;
using Application.MathObjects.Matrices;

namespace Application.FEM._2D.Assembling.Local;

public class RotorLocalMatricesAssembler2D : ICylindricalLocalMatricesAssembler<IElement2D, double>
{
    private readonly ICylindricalLocalStiffnessMatrixAssembler<IElement2D, double> _cylindricalLocalStiffnessMatrixAssembler;
    private readonly ICylindricalLocalMassMatrixAssembler<IElement2D, double> _cylindricalLocalMassMatrixAssembler;

    public RotorLocalMatricesAssembler2D
    (
        ICylindricalLocalStiffnessMatrixAssembler<IElement2D, double> cylindricalLocalStiffnessMatrixAssembler,
        ICylindricalLocalMassMatrixAssembler<IElement2D, double> cylindricalLocalMassMatrixAssembler
    )
    {
        _cylindricalLocalStiffnessMatrixAssembler = cylindricalLocalStiffnessMatrixAssembler;
        _cylindricalLocalMassMatrixAssembler = cylindricalLocalMassMatrixAssembler;
    }

    public IMatrix<double> AssembleStiffnessMatrix(IElement2D element, double r) =>
        _cylindricalLocalStiffnessMatrixAssembler.AssembleStiffnessMatrix(element, r);

    public IMatrix<double> AssembleMassMatrix(IElement2D element, double r) =>
        _cylindricalLocalMassMatrixAssembler.AssembleMassMatrix(element, r);
}