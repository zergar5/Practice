using System.Numerics;
using Application.FEM.Core.Assembling.Local;
using Services.MathObjects.Matrices;
using Services.MathObjects.Vectors;

namespace Application.FEM.Core.Assembling.Inserters;

public interface IGenericInserter<T, in TMatrix> where T : INumber<T>
{
    public void InsertMatrix(TMatrix globalMatrix, ILocalMatrix<T> localMatrix);
    public void InsertVector(IVector<T> globalVector, ILocalVector<T> localVector);
}

public interface IInserter<T> : IGenericInserter<T, IMatrix<T>> where T : INumber<T>;