using Application.FEM.Core.Assembling.Local;
using Application.MathObjects.Matrices;
using Application.MathObjects.Vectors;
using System.Numerics;

namespace Application.FEM.Core.Assembling.Inserters;

public interface IGenericInserter<T, in TMatrix> where T : INumberBase<T>
{
    public void InsertMatrix(TMatrix globalMatrix, ILocalMatrix<T> localMatrix);
    public void InsertVector(IVector<T> globalVector, ILocalVector<T> localVector);
}