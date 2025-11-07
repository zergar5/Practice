using Application.MathObjects.Matrices;

namespace Application.FEM._1D.MatrixTemplates;

public static class LagrangeMatrixTemplates
{
    public static IMatrix<int> MassMatrix => new Matrix<int>
    (
        new[,]
        {
            { 2, 1 },
            { 1, 2 }
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