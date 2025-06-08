namespace Application.EquationSystems.MatrixDecompositions;

public interface IMatrixDecomposition<TMatrix>
{
    public TMatrix Decompose(TMatrix matrix);
}