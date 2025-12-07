using Application.FEM.Core.Assembling.Local;
using Application.MathObjects.Vectors;

namespace Application.FEM.Core.Assembling.Inserters;

public interface IGenericInserter<in TMatrix>
{
    public void InsertMatrix(TMatrix globalMatrix, ILocalMatrix<double> localMatrix);
    public void InsertVector(IVector<double> globalVector, ILocalVector<double> localVector);
}