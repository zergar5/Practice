using DirectProblem.Core.Base;
using DirectProblem.Core.Global;

namespace DirectProblem.SLAE.Solvers;

public class LUSparse
{
    public Vector CalcY(SparseMatrix sparseMatrix, Vector b, Vector? y = null)
    {
        y ??= new Vector(b.Count);

        for (var i = 0; i < sparseMatrix.Count; i++)
        {
            var sum = 0.0;

            foreach (var j in sparseMatrix[i])
            {
                sum += sparseMatrix[i, j] * y[j];
            }

            y[i] = (b[i] - sum) / sparseMatrix[i, i];
        }

        return y;
    }

    public Vector CalcX(SparseMatrix sparseMatrix, Vector y, Vector? x = null)
    {
        x = x == null ? y.Clone() : y.Copy(x);

        for (var i = sparseMatrix.Count - 1; i >= 0; i--)
        {
            var columns = sparseMatrix[i];
            for (var j = columns.Length - 1; j >= 0; j--)
            {
                x[columns[j]] -= sparseMatrix[columns[j], i] * x[i];
            }
        }

        return x;
    }
}