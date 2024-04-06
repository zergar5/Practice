using DirectProblem.Core.Global;

namespace DirectProblem.SLAE.Preconditions;

public class LUPreconditioner : IPreconditioner<SparseMatrix>
{
    public SparseMatrix Decompose(SparseMatrix globalMatrix)
    {
        var preconditionMatrix = globalMatrix;

        for (var i = 0; i < preconditionMatrix.Count; i++)
        {
            var sumD = 0.0;

            for (var j = preconditionMatrix.RowsIndexes[i]; j < preconditionMatrix.RowsIndexes[i + 1]; j++)
            {
                var sumL = 0d;
                var sumU = 0d;

                for (var k = preconditionMatrix.RowsIndexes[i]; k < j; k++)
                {
                    var iPrev = i - preconditionMatrix.ColumnsIndexes[j];
                    var kPrev = preconditionMatrix.IndexOf(i - iPrev, preconditionMatrix.ColumnsIndexes[k]);

                    if (kPrev == -1) continue;

                    sumL += preconditionMatrix[i, preconditionMatrix.ColumnsIndexes[k]] *
                            preconditionMatrix[preconditionMatrix.ColumnsIndexes[kPrev], i - iPrev];
                    sumU += preconditionMatrix[preconditionMatrix.ColumnsIndexes[k], i] *
                            preconditionMatrix[i - iPrev, preconditionMatrix.ColumnsIndexes[kPrev]];
                }

                var jColumn = preconditionMatrix.ColumnsIndexes[j];

                preconditionMatrix[i, jColumn] -= sumL;
                preconditionMatrix[jColumn, i] = (preconditionMatrix[jColumn, i] - sumU) /
                                                 preconditionMatrix[jColumn, jColumn];

                sumD += preconditionMatrix[i, jColumn] * preconditionMatrix[jColumn, i];
            }

            preconditionMatrix[i, i] -= sumD;
        }

        return preconditionMatrix;
    }
}