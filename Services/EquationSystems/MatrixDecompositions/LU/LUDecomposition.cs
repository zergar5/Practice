using Application.MathObjects.Matrices;

namespace Application.EquationSystems.MatrixDecompositions.LU;

public class LUDecomposition : IMatrixDecomposition<IProfileMatrix<double>>
{
    public IProfileMatrix<double> Decompose(IProfileMatrix<double> matrix)
    {
        for (var i = 0; i < matrix.RowCount; i++)
        {
            var j = i - (matrix.RowIndexes[i + 1] - matrix.RowIndexes[i]);

            var sumD = 0d;

            for (var ij = matrix.RowIndexes[i]; ij < matrix.RowIndexes[i + 1]; ij++, j++)
            {
                var sumL = 0d;
                var sumU = 0d;

                var k = j - (matrix.RowIndexes[j + 1] - matrix.RowIndexes[j]);

                var ik = matrix.RowIndexes[i];
                var kj = matrix.RowIndexes[j];

                if (k - (i - (matrix.RowIndexes[i + 1] - matrix.RowIndexes[i])) < 0) kj -= k - (i - (matrix.RowIndexes[i + 1] - matrix.RowIndexes[i]));
                else ik += k - (i - (matrix.RowIndexes[i + 1] - matrix.RowIndexes[i]));

                for (; ik < ij; ik++, kj++)
                {
                    sumL += matrix.LowerValues[ik] * matrix.UpperValues[kj];
                    sumU += matrix.LowerValues[kj] * matrix.UpperValues[ik];
                }

                matrix.LowerValues[ij] -= sumL;
                matrix.UpperValues[ij] = (matrix.UpperValues[ij] - sumU) / matrix.Diagonal[j];

                sumD += matrix.LowerValues[ij] * matrix.UpperValues[ij];
            }

            matrix.Diagonal[i] -= sumD;
        }

        return matrix;
    }
}