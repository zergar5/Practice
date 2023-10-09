using DirectProblem.Core.Base;

namespace DirectProblem.Core.Local;

public class LocalMatrix
{
    public int[] Indexes { get; }
    public Matrix Matrix { get; }

    public LocalMatrix(int[] indexes, Matrix matrix)
    {
        Matrix = matrix;
        Indexes = indexes;
    }

    public double this[int i, int j]
    {
        get => Matrix[i, j];
        set => Matrix[i, j] = value;
    }

    public static LocalMatrix Multiply(double coefficient, LocalMatrix matrix)
    {
        for (var i = 0; i < matrix.Matrix.CountRows; i++)
        {
            for (var j = 0; j < matrix.Matrix.CountColumns; j++)
            {
                matrix[i, j] *= coefficient;
            }
        }

        return matrix;
    }
}