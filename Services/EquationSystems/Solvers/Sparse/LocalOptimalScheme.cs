using Application.EquationSystems.Preconditions.Separate;
using Application.MathObjects.Equation;
using Application.MathObjects.Matrices;
using Application.MathObjects.Vectors;

namespace Application.EquationSystems.Solvers.Sparse;

public class LocalOptimalScheme : ISparseSLAESolver
{
    private readonly ISeparatePrecondition<ISparseMatrix<double>> _precondition;
    private readonly IterativeMethodConfig _config;

    public LocalOptimalScheme(ISeparatePrecondition<ISparseMatrix<double>> precondition, IterativeMethodConfig config)
    {
        _precondition = precondition;
        _config = config;
    }

    public IVector<double> Solve(IEquation<ISparseMatrix<double>, double> equation)
    {
        var matrix = equation.Matrix;
        var solution = equation.Solution;
        var rightPart = equation.RightPart;

        _precondition.DecomposeMatrix(equation.Matrix.Clone());

        var r = matrix.Multiply(solution, VectorPool<double>.Rent(solution.Count));

        _precondition.ForwardElimination(rightPart.Subtract(r, r), r);

        var z = _precondition.BackSubstitution(r, VectorPool<double>.Rent(r.Count));
        var p = matrix.Multiply(z, VectorPool<double>.Rent(z.Count));

        _precondition.ForwardElimination(p, p);

        var residual = r.ScalarProduct();
        var residualNext = residual;
        var targetResidual = Math.Pow(_config.ResidualPrecision, 2);

        var firstBufferVector = VectorPool<double>.Rent(z.Count);
        var secondBufferVector = VectorPool<double>.Rent(firstBufferVector.Count);

        var i = 1;

        for (; i <= _config.MaxIterations; i++)
        {
            var pScalarProduct = p.ScalarProduct();
            var alpha = p.ScalarProduct(r) / pScalarProduct;

            solution.Sum(z.Multiply(alpha, firstBufferVector), solution);

            var rNext = r.Subtract(p.Multiply(alpha, firstBufferVector), r);

            residualNext = rNext.ScalarProduct() / residual;

            if (residualNext <= targetResidual) return solution;

            _precondition.BackSubstitution(rNext, firstBufferVector);

            var LAUr = _precondition.ForwardElimination(matrix.Multiply(firstBufferVector, secondBufferVector), firstBufferVector);

            var beta = -(p.ScalarProduct(LAUr) / pScalarProduct);

            var zNext = _precondition.BackSubstitution(rNext, secondBufferVector).Sum(z.Multiply(beta, z), z);
            var pNext = LAUr.Sum(p.Multiply(beta, p), p);

            r = rNext;
            z = zNext;
            p = pNext;

            //CourseHolder.GetResidualInfo(i, residualNext);
        }

        if (residualNext > targetResidual)
        {
            Console.WriteLine($"Iterations exceeded with residual {residualNext}");
            Console.WriteLine();
        }

        VectorPool<double>.Return(r);
        VectorPool<double>.Return(z);
        VectorPool<double>.Return(p);
        VectorPool<double>.Return(firstBufferVector);
        VectorPool<double>.Return(secondBufferVector);

        return solution;
    }
}