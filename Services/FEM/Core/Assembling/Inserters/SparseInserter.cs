using Application.FEM.Core.Assembling.Local;
using Application.MathObjects.Matrices;
using Application.MathObjects.Vectors;

namespace Application.FEM.Core.Assembling.Inserters;

public interface ISparseInserter : IGenericInserter<ISparseMatrix<double>>;

public abstract class SparseInserterBase : ISparseInserter
{
    public abstract void InsertMatrix(ISparseMatrix<double> globalMatrix, ILocalMatrix<double> localMatrix);
    public virtual void InsertVector(IVector<double> globalVector, ILocalVector<double> localVector)
    {
        for (var i = 0; i < localVector.Count; i++)
        {
            globalVector[localVector.GetGlobalIndexOfLocal(i)] += localVector[i];
        }
    }
}

public class SparseInserter : SparseInserterBase
{
    public override void InsertMatrix(ISparseMatrix<double> globalMatrix, ILocalMatrix<double> localMatrix)
    {
        for (var i = 0; i < localMatrix.RowCount; i++)
        {
            var rowIndex = localMatrix.GetGlobalIndexOfLocal(i);

            for (var j = 0; j < i; j++)
            {
                var columnIndex = localMatrix.GetGlobalIndexOfLocal(j);

                globalMatrix[rowIndex, columnIndex] += localMatrix[i, j];
                globalMatrix[columnIndex, rowIndex] += localMatrix[j, i];
            }

            globalMatrix[rowIndex, rowIndex] += localMatrix[i, i];
        }
    }
}