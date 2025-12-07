using Application.MathObjects.Equation;
using Application.MathObjects.Matrices;
using System.Numerics;

namespace Application.FEM.Core.Assembling.Boundaries.First;

public interface IFirstBoundaryApplier<in TMatrix, in TBoundary> where TBoundary : INumberBase<TBoundary>
{
    public void Exclude(IEquation<TMatrix, double> equation, IFirstBoundaryValue<TBoundary> conditionValue);
}

public class SparseMatrixFirstBoundaryApplier<TBoundary> : IFirstBoundaryApplier<ISparseMatrix<double>, TBoundary>
    where TBoundary : INumberBase<TBoundary>
{
    public virtual void Exclude(IEquation<ISparseMatrix<double>, double> equation, IFirstBoundaryValue<TBoundary> conditionValue)
    {
        var row = conditionValue.NodeId;

        ExcludeRow(equation, row, double.CreateChecked(conditionValue.Value));
    }

    protected void ExcludeRow(IEquation<ISparseMatrix<double>, double> equation, int row, double valueForRightPart)
    {
        equation.RightPart[row] = valueForRightPart;
        equation.Matrix[row, row] = 1d;

        foreach (var j in equation.Matrix[row])
        {
            equation.Matrix[row, j] = 0d;
        }

        for (var column = row + 1; column < equation.Matrix.ColumnCount; column++)
        {
            if (!equation.Matrix[column].Contains(row)) continue;

            equation.Matrix[row, column] = 0d;
        }
    }
}

public class ComplexFirstBoundaryApplier : SparseMatrixFirstBoundaryApplier<Complex>
{
    public override void Exclude(IEquation<ISparseMatrix<double>, double> equation, IFirstBoundaryValue<Complex> conditionValue)
    {
        var complexRow = 2 * conditionValue.NodeId;

        ExcludeRow(equation, complexRow, conditionValue.Value.Real);
        ExcludeRow(equation, complexRow + 1, conditionValue.Value.Imaginary);
    }
}