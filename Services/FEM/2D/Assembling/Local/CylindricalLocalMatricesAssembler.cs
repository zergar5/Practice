using Application.FEM.Core.Assembling.Local;
using Application.MathObjects.Matrices;

namespace Application.FEM._2D.Assembling.Local;

public class RotorLocalMatricesAssembler2D : ICylindricalLocalMatricesAssembler<IElement2D>
{
    private readonly ICylindricalLocalStiffnessMatrixAssembler<IElement2D> _cylindricalLocalStiffnessMatrixAssembler;
    private readonly ICylindricalLocalMassMatrixAssembler<IElement2D> _cylindricalLocalMassMatrixAssembler;

    public RotorLocalMatricesAssembler2D
    (
        ICylindricalLocalStiffnessMatrixAssembler<IElement2D> cylindricalLocalStiffnessMatrixAssembler,
        ICylindricalLocalMassMatrixAssembler<IElement2D> cylindricalLocalMassMatrixAssembler
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