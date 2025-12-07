using Application.MathObjects.Vectors;

namespace Application.EquationSystems.Preconditions.Separate;

public interface ISeparatePrecondition<in TMatrix>
{
    public void DecomposeMatrix(TMatrix matrix);
    public IVector<double> ForwardElimination(IVector<double> vector, IVector<double>? result = null);
    public IVector<double> BackSubstitution(IVector<double> vector, IVector<double>? result = null);
}