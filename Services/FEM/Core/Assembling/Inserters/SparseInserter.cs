using Application.FEM.Core.Assembling.Local;
using Application.MathObjects.Matrices;
using Application.MathObjects.Vectors;
using System.Numerics;

namespace Application.FEM.Core.Assembling.Inserters;

public interface ISparseInserter<T> : IGenericInserter<T, ISparseMatrix<T>> where T : INumberBase<T>;

public abstract class SparseInserterBase<T> : ISparseInserter<T> where T : INumberBase<T>
{
    public abstract void InsertMatrix(ISparseMatrix<T> globalMatrix, ILocalMatrix<T> localMatrix);
    public virtual void InsertVector(IVector<T> globalVector, ILocalVector<T> localVector)
    {
        for (var i = 0; i < localVector.Count; i++)
        {
            globalVector[localVector.GetGlobalIndexOfLocal(i)] += localVector[i];
        }
    }
}

public class SparseInserter<T> : SparseInserterBase<T> where T : INumberBase<T>
{
    public override void InsertMatrix(ISparseMatrix<T> globalMatrix, ILocalMatrix<T> localMatrix)
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