using System.Numerics;
using Application.MathObjects.Matrices;
using DirectProblem.SLAE.Preconditions;

namespace Application.EquationSystems.MatrixDecompositions.LU;

public class LUIncompleteDecomposition<T> : IPreconditioner<ISparseMatrix<T>> where T : INumberBase<T>
{
    public ISparseMatrix<T> Decompose(ISparseMatrix<T> matrix)
    {
        for (var i = 0; i < matrix.RowCount; i++)
        {
            var sumD = T.Zero;
            var rowColumns = matrix[i];

            for (var j = 0; j < rowColumns.Length; j++)
            {
                var sumL = T.Zero;
                var sumU = T.Zero;
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