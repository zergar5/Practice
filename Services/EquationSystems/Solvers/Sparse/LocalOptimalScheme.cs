using Application.EquationSystems.Preconditions.Separate;
using Application.MathObjects.Equation;
using Application.MathObjects.Matrices;
using Application.MathObjects.Vectors;
using System.Numerics;
using DirectProblem.FEM;

namespace Application.EquationSystems.Solvers.Sparse;

public class LocalOptimalScheme<T> : ISparseSLAESolver<T> where T : INumberBase<T>
{
    private readonly ISeparatePrecondition<ISparseMatrix<T>, T> _precondition;
    private readonly IterativeMethodConfig _config;

    public LocalOptimalScheme(ISeparatePrecondition<ISparseMatrix<T>, T> precondition, IterativeMethodConfig config)
    {
        _precondition = precondition;
        _config = config;
    }

    public IVector<T> Solve(IEquation<ISparseMatrix<T>, T> equation)
    {
        var matrix = equation.Matrix;
        var solution = equation.Solution;
        var rightPart = equation.RightPart;

        _precondition.DecomposeMatrix(equation.Matrix.Clone());

        var r = matrix.Multiply(solution, VectorPool<T>.Rent(solution.Count));

        _precondition.ForwardElimination(rightPart.Subtract(r, r), r);

        var z = _precondition.BackSubstitution(r, VectorPool<T>.Rent(r.Count));
        var p = matrix.Multiply(z, VectorPool<T>.Rent(z.Count));

        _precondition.ForwardElimination(p, p);

        var residual = r.ScalarProduct();
        var targetResidual = Math.Pow(_config.ResidualPrecision, 2);

        var firstBufferVector = VectorPool<T>.Rent(z.Count);
        var secondBufferVector = VectorPool<T>.Rent(firstBufferVector.Count);

        for (var i = 1; i <= _config.MaxIterations; i++)
        {
            var pScalarProduct = p.ScalarProduct();
            var alpha = p.ScalarProduct(r) / pScalarProduct;

            solution.Sum(z.Multiply(alpha, firstBufferVector), solution);

            var rNext = r.Subtract(p.Multiply(alpha, firstBufferVector), r);
            var residualNext = rNext.ScalarProduct() / residual;

            if (residualNext <= targetResidual) return solution;

            _precondition.BackSubstitution(rNext, firstBufferVector);

            var LAUr = _precondition.ForwardElimination(matrix.Multiply(firstBufferVector, secondBufferVector), firstBufferVector);

            var beta = -(p.ScalarProduct(LAUr) / pScalarProduct);

            var zNext = _precondition.BackSubstitution(rNext, secondBufferVector).Sum(z.Multiply(beta, z), z);
            var pNext = LAUr.Sum(p.Multiply(beta, p), p);

            r = rNext;
            z = zNext;
            p = pNext;

            CourseHolder.GetResidualInfo(i, residualNext);
        }

        VectorPool<T>.Return(r);
        VectorPool<T>.Return(z);
        VectorPool<T>.Return(p);
        VectorPool<T>.Return(firstBufferVector);
        VectorPool<T>.Return(secondBufferVector);

        return solution;
    }
}