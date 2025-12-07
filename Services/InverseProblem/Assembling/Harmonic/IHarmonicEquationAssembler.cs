using Application.InverseProblem.Parameters;
using Application.MathObjects.Equation;
using Application.MathObjects.Matrices;

namespace Application.InverseProblem.Assembling.Harmonic;

public interface IHarmonicEquationAssembler
{
    public IHarmonicEquationAssembler AllocateEquation(Parameter[] parameters, double[,] targetMeasurements, double[,] weightSquares);
    public IEquation<IMatrix<double>, double> Assemble(double[,] measurements);
}