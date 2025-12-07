using Application.MathObjects.Matrices;

namespace Application.EquationSystems.Solvers.Sparse;

public interface ISparseSLAESolver : ISLAESolver<ISparseMatrix<double>>;