using Application.MathObjects.Matrices;
using System.Numerics;

namespace Application.EquationSystems.Solvers.Sparse;

public interface ISparseSLAESolver<T> : ISLAESolver<ISparseMatrix<T>, T> where T : INumberBase<T>;