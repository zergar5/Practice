using Application.FEM.Core.Assembling.Local;
using System.Numerics;
using Application.MathObjects.Matrices;
using Application.MathObjects.Vectors;

namespace Application.FEM.Core.Assembling.Inserters;

public interface ISparseInserter<T> : IGenericInserter<T, ISparseMatrix<T>> where T : INumberBase<T>;

public abstract class SparseInserterBase<T> : ISparseInserter<T> where T : INumberBase<T>
{
    public abstract void InsertMatrix(ISparseMatrix<T> globalMatrix, ILocalMatrix<T> localMatrix);
    public virtual void InsertVector(IVector<T> globalVector, ILocalVector<T> localVector)
    {
        for (var i = 0; i < localVector.Count; i++)
        {
            var indexedValue = localVector[i];
            globalVector[indexedValue.Key] += indexedValue.Value;
        }
    }
}

public class SparseInserter<T> : SparseInserterBase<T> where T : INumberBase<T>
{
    public override void InsertMatrix(ISparseMatrix<T> globalMatrix, ILocalMatrix<T> localMatrix)
    {
        for (var i = 0; i < localMatrix.RowCount; i++)
        {
            for (var j = 0; j < i; j++)
            {
                var indexedValue = localMatrix[i, j];
                var (rowIndex, columnIndex) = indexedValue.Key;

                globalMatrix[rowIndex, columnIndex] += indexedValue.Value;
                globalMatrix[columnIndex, rowIndex] += indexedValue.Value;
            }

            var indexedDiagonalValue = localMatrix[i, i];
            var (elementIndex, _) = indexedDiagonalValue.Key;

            globalMatrix[elementIndex, elementIndex] += indexedDiagonalValue.Value;
        }
    }
}