using Application.MathObjects.Matrices;

namespace Application.FEM._1D.MatrixTemplates;

public class CylindricalMatrixTemplates
{
    public static IMatrix<int> AdditionMassMatrix => new Matrix<int>
    (
        new[,]
        {
            { 1, 1 },
            { 1, 3 }
        }
    );

    public static IMatrix<int> RotorAdditionMassMatrix => new Matrix<int>
    (
        new[,]
        {
            { -3, 1 },
            { 1, 1 }
        }
    );
}