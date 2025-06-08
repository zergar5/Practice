using System.Numerics;
using Application.MathObjects.Matrices;

namespace Application.EquationSystems.Solvers.Sparse;

public interface ISparseSLAESolver<T> : ISLAESolver<ISparseMatrix<T>, T> where T : INumberBase<T>;