using Application.MathObjects.Equation;
using Application.MathObjects.Matrices;
using System.Numerics;

namespace Application.FEM.Core.Assembling.Boundaries.First;

public interface IFirstBoundaryApplier<in TMatrix, T, in TBoundary>
    where T : INumberBase<T>
    where TBoundary : INumberBase<TBoundary>
{
    public void Exclude(IEquation<TMatrix, T> equation, IFirstBoundaryValue<TBoundary> conditionValue);
}

public class SparseMatrixFirstBoundaryApplier<T, TBoundary> : IFirstBoundaryApplier<ISparseMatrix<T>, T, TBoundary>
    where T : INumberBase<T>
    where TBoundary : INumberBase<TBoundary>
{
    public virtual void Exclude(IEquation<ISparseMatrix<T>, T> equation, IFirstBoundaryValue<TBoundary> conditionValue)
    {
        var row = conditionValue.NodeId;

        ExcludeRow(equation, row, T.CreateChecked(conditionValue.Value));
    }

    protected void ExcludeRow(IEquation<ISparseMatrix<T>, T> equation, int row, T valueForRightPart)
    {
        equation.RightPart[row] = T.CreateChecked(valueForRightPart);
        equation.Matrix[row, row] = T.One;

        foreach (var j in equation.Matrix[row])
        {
            equation.Matrix[row, j] = T.Zero;
        }

        for (var column = row + 1; column < equation.Matrix.ColumnCount; column++)
        {
            if (!equation.Matrix[column].Contains(row)) continue;

            equation.Matrix[row, column] = T.Zero;
        }
    }
}

public class ComplexFirstBoundaryApplier : SparseMatrixFirstBoundaryApplier<double, Complex>
{
    public override void Exclude(IEquation<ISparseMatrix<double>, double> equation, IFirstBoundaryValue<Complex> conditionValue)
    {
        var complexRow = 2 * conditionValue.NodeId;

        ExcludeRow(equation, complexRow, conditionValue.Value.Real);
        ExcludeRow(equation, complexRow + 1, conditionValue.Value.Imaginary);
    }
}