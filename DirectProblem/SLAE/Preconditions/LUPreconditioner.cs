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
                    var kPrev = preconditionMatrix[i - iPrev].IndexOf(preconditionMatrix.ColumnsIndexes[k]);

                    if (kPrev == -1) continue;

                    sumL += preconditionMatrix[iPrev, preconditionMatrix.ColumnsIndexes[k]] *
                            preconditionMatrix[preconditionMatrix.ColumnsIndexes[kPrev], iPrev];
                    sumU += preconditionMatrix[preconditionMatrix.ColumnsIndexes[k], iPrev] *
                            preconditionMatrix[iPrev, preconditionMatrix.ColumnsIndexes[kPrev]];
                }

                preconditionMatrix[i, preconditionMatrix.ColumnsIndexes[j]] -= sumL;
                preconditionMatrix[preconditionMatrix.ColumnsIndexes[j], i] =
                    (preconditionMatrix[preconditionMatrix.ColumnsIndexes[j], i] - sumU) /
                    preconditionMatrix[preconditionMatrix.ColumnsIndexes[j], preconditionMatrix.ColumnsIndexes[j]];

                sumD += preconditionMatrix[i, preconditionMatrix.ColumnsIndexes[j]] *
                        preconditionMatrix[preconditionMatrix.ColumnsIndexes[j], i];
            }

            preconditionMatrix[i, i] -= sumD;
        }

        return preconditionMatrix;
    }
}