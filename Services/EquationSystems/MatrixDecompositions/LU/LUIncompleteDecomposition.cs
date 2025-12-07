using Application.MathObjects.Matrices;

namespace Application.EquationSystems.MatrixDecompositions.LU;

public class LUIncompleteDecomposition : IMatrixDecomposition<ISparseMatrix<double>>
{
    public ISparseMatrix<double> Decompose(ISparseMatrix<double> matrix)
    {
        for (var i = 0; i < matrix.RowCount; i++)
        {
            var sumD = 0d;
            var rowColumns = matrix[i];

            for (var j = 0; j < rowColumns.Length; j++)
            {
                var sumL = 0d;
                var sumU = 0d;
                var jColumn = rowColumns[j];

                for (var k = 0; k < j; k++)
                {
                    var iPrevious = i - (i - jColumn);

                    if (!matrix[iPrevious].Contains(rowColumns[k])) continue;

                    var kColumn = rowColumns[k];

                    sumL += matrix[i, kColumn] * matrix[kColumn, iPrevious];
                    sumU += matrix[kColumn, i] * matrix[iPrevious, kColumn];
                }

                matrix[i, jColumn] -= sumL;
                matrix[jColumn, i] = (matrix[jColumn, i] - sumU) / matrix[jColumn, jColumn];

                sumD += matrix[i, jColumn] * matrix[jColumn, i];
            }

            matrix[i, i] -= sumD;
        }

        return matrix;
    }
}