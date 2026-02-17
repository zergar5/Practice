using Application.EquationSystems.MatrixDecompositions;
using Application.MathObjects.Equation;
using Application.MathObjects.Matrices;
using Application.MathObjects.Vectors;
using DirectProblem.Core.Global;

namespace Application.EquationSystems.Solvers.Sparse;

public class LU : ISparseSLAESolver
{
    private readonly IMatrixDecomposition<IProfileMatrix<double>> _matrixDecomposition;

    public LU(IMatrixDecomposition<IProfileMatrix<double>> matrixDecomposition)
    {
        _matrixDecomposition = matrixDecomposition;
    }

    public IVector<double> Solve(IEquation<ISparseMatrix<double>, double> equation)
    {
        var matrix = _matrixDecomposition.Decompose(equation.Matrix.ToProfileMatrix());
        var solution = ForwardElimination(matrix, equation.Solution, equation.RightPart);

        solution = BackSubstitution(matrix, solution);

        return solution;
    }

    private static IVector<double> ForwardElimination(IProfileMatrix<double> matrix, IVector<double> solution, IVector<double> rightPart)
    {
        for (var i = 0; i < matrix.RowCount; i++)
        {
            var sum = 0d;
            var k = i - (matrix.RowIndexes[i + 1] - matrix.RowIndexes[i]);

            for (var j = matrix.RowIndexes[i]; j < matrix.RowIndexes[i + 1]; j++, k++)
            {
                sum += matrix.LowerValues[j] * solution[k];
            }

            solution[i] = (rightPart[i] - sum) / matrix.Diagonal[i];
        }

        return solution;
    }

    private static IVector<double> BackSubstitution(IProfileMatrix<double> matrix, IVector<double> solution)
    {
        for (var i = matrix.RowCount - 1; i >= 0; i--)
        {
            var k = i - 1;

            for (var j = matrix.RowIndexes[i + 1] - 1; j >= matrix.RowIndexes[i]; j--, k--)
            {
                solution[k] -= matrix.UpperValues[j] * solution[i];
            }
        }

        return solution;
    }
}