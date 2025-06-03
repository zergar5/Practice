using Services.MathObjects.Matrices;

namespace Application.FEM._1D.MatrixTemplates;

public class LagrangeMatrixTemplates
{
    public static IMatrix<int> MassMatrix => new Matrix<int>
    (
        new[,]
        {
            { 2, 1 },
            { 1, 2 }
        }
    );

    public static IMatrix<int> MassRMatrix => new Matrix<int>
    (
        new[,]
        {
            { 1, 1 },
            { 1, 3 }
        }
    );

    public static IMatrix<int> RotorMassMatrix => new Matrix<int>
    (
        new[,]
        {
            { -3, 1 },
            { 1, 1 }
        }
    );

    public static IMatrix<int> StiffnessMatrix => new Matrix<int>
    (
        new[,]
        {
            { 1, -1 },
            { -1, 1 }
        }
    );
}