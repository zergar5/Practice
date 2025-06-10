using Application.MathObjects.Equation;
using Application.MathObjects.Matrices;
using System.Numerics;

namespace Application.FEM.Core.Assembling.Boundaries.First;

public interface IFirstBoundaryApplier<in TMatrix, T, TBoundary>
    where T : INumberBase<T>
    where TBoundary : INumberBase<TBoundary>
{
    public void Exclude(IEquation<TMatrix, T> equation, FirstBoundaryValue<TBoundary> conditionValue);
}

public abstract class SparseMatrixFirstBoundaryApplierBase<T, TBoundary> : IFirstBoundaryApplier<SparseMatrix<T>, T, TBoundary>
    where T : INumberBase<T>
    where TBoundary : INumberBase<TBoundary>
{
    public virtual void Exclude(IEquation<SparseMatrix<T>, T> equation, FirstBoundaryValue<TBoundary> conditionValue)
    {
        var row = conditionValue.NodeId;
        equation.RightPart[row] = T.CreateChecked(conditionValue.Value);
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

public class ComplexFirstBoundaryApplier : SparseMatrixFirstBoundaryApplierBase<double, Complex>
{
    public override void Exclude(IEquation<SparseMatrix<double>, double> equation, FirstBoundaryValue<Complex> conditionValue)
    {
        throw new NotImplementedException();
    }
}