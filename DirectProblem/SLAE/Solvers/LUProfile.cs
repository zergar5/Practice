using DirectProblem.Core.Base;
using DirectProblem.Core.Global;
using Practice6Sem.Core.Global;

namespace DirectProblem.SLAE.Solvers;

public class LUProfile
{
    public Vector Solve(Equation<ProfileMatrix> equation)
    {
        var matrix = equation.Matrix.LU();
        var y = CalcY(matrix, equation.Solution, equation.RightPart);
        var x = CalcX(matrix, y);

        return x;
    }

    private Vector CalcY(ProfileMatrix profileMatrix, Vector q, Vector b)
    {
        var y = q;

        for (var i = 0; i < profileMatrix.Count; i++)
        {
            var sum = 0d;

            var k = i - (profileMatrix.RowsIndexes[i + 1] - profileMatrix.RowsIndexes[i]);

            for (var j = profileMatrix.RowsIndexes[i]; j < profileMatrix.RowsIndexes[i + 1]; j++, k++)
            {
                sum += profileMatrix.LowerValues[j] * y[k];
            }

            y[i] = (b[i] - sum) / profileMatrix.Diagonal[i];
        }

        return y;
    }

    private Vector CalcX(ProfileMatrix profileMatrix, Vector y)
    {
        var x = y;

        for (var i = profileMatrix.Count - 1; i >= 0; i--)
        {
            var k = i - 1;

            for (var j = profileMatrix.RowsIndexes[i + 1] - 1; j >= profileMatrix.RowsIndexes[i]; j--, k--)
            {
                x[k] -= profileMatrix.UpperValues[j] * x[i];
            }
        }

        return x;
    }
}