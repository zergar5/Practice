using Application.EquationSystems.MatrixDecompositions;
using Application.MathObjects.Matrices;
using Application.MathObjects.Vectors;

namespace Application.EquationSystems.Preconditions.Separate;

public class LUPrecondition : ISeparatePrecondition<ISparseMatrix<double>>
{
    private readonly IMatrixDecomposition<ISparseMatrix<double>> _matrixDecomposition;
    private ISparseMatrix<double> _decomposedMatrix;

    public LUPrecondition(IMatrixDecomposition<ISparseMatrix<double>> matrixDecomposition)
    {
        _matrixDecomposition = matrixDecomposition;
    }

    public void DecomposeMatrix(ISparseMatrix<double> matrix)
    {
        _decomposedMatrix = _matrixDecomposition.Decompose(matrix);
    }

    public IVector<double> ForwardElimination(IVector<double> vector, IVector<double>? result = null)
    {
        if (_decomposedMatrix.RowCount != vector.Count)
            throw new ArgumentOutOfRangeException($"{nameof(_decomposedMatrix)} and {nameof(vector)} must have same size");

        result ??= new Vector<double>(vector.Count);

        for (var i = 0; i < _decomposedMatrix.RowCount; i++)
        {
            var sum = 0d;

            foreach (var j in _decomposedMatrix[i])
            {
                sum += _decomposedMatrix[i, j] * result[j];
            }

            result[i] = (vector[i] - sum) / _decomposedMatrix[i, i];
        }

        return result;
    }

    public IVector<double> BackSubstitution(IVector<double> vector, IVector<double>? result = null)
    {
        result = result == null ? vector.Clone() : vector.Copy(result);

        for (var i = _decomposedMatrix.RowCount - 1; i >= 0; i--)
        {
            var columns = _decomposedMatrix[i];
            for (var j = columns.Length - 1; j >= 0; j--)
            {
                result[columns[j]] -= _decomposedMatrix[columns[j], i] * result[i];
            }
        }

        return result;
    }
}