using Application.MathObjects.Matrices;
using Application.MathObjects.Vectors;
using System.Numerics;

namespace Application.EquationSystems.Preconditions.Separate;

public class LUPrecondition<T> : ISeparatePrecondition<T> where T : INumberBase<T>
{
    private readonly ISparseMatrix<T> _decomposedMatrix;

    public LUPrecondition(ISparseMatrix<T> decomposedMatrix)
    {
        _decomposedMatrix = decomposedMatrix;
    }

    public IVector<T> ForwardElimination(IVector<T> vector, IVector<T>? result = null)
    {
        if (_decomposedMatrix.RowCount != vector.Count)
            throw new ArgumentOutOfRangeException($"{nameof(_decomposedMatrix)} and {nameof(vector)} must have same size");

        result ??= new MathObjects.Vectors.Vector<T>(vector.Count);

        for (var i = 0; i < _decomposedMatrix.RowCount; i++)
        {
            var sum = T.Zero;

            foreach (var j in _decomposedMatrix[i])
            {
                sum += _decomposedMatrix[i, j] * result[j];
            }

            result[i] = (vector[i] - sum) / _decomposedMatrix[i, i];
        }

        return result;
    }

    public IVector<T> BackSubstitution(IVector<T> vector, IVector<T>? result = null)
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