using System.Numerics;
using Application.EquationSystems.Preconditions.Separate;
using Application.MathObjects.Equation;
using Application.MathObjects.Matrices;
using Application.MathObjects.Vectors;
using DirectProblem.Core.Global;
using DirectProblem.SLAE.Preconditions;
using DirectProblem.SLAE.Solvers;
using DirectProblem.SLAE;

namespace Application.EquationSystems.Solvers.Sparse;

public class LocalOptimalScheme<T> : ISparseSLAESolver<T> where T : INumberBase<T>
{
    private readonly ISeparatePrecondition<T> _precondition;

    public LocalOptimalScheme(ISeparatePrecondition<T> precondition)
    {
        _precondition = precondition;
    }

    public IVector<T> Solve(IEquation<ISparseMatrix<T>, T> equation)
    {
        var matrix = equation.Matrix;
        var solution = equation.Solution;
        var rightPart = equation.RightPart;

        var r = matrix.Multiply(solution);
        _precondition.ForwardElimination( rightPart.Subtract(r, r), r);
        var z = _precondition.BackSubstitution(r);
        var p = matrix.Multiply(matrix, z, p);
        _precondition.ForwardElimination( p, p);

        var residual = r.ScalarProduct();
        var residualNext = residual;
        // TODO вынести выделения буфферов в одно место если получится
        var firstBufferVector = new MathObjects.Vectors.Vector<T>(solution.Count);
        var secondBufferVector = new MathObjects.Vectors.Vector<T>(solution.Count);

        for (var i = 1; i <= MethodsConfig.MaxIterations && residualNext > Math.Pow(MethodsConfig.ResidualPrecision, 2); i++)
        {
            var pScalarProduct = p.ScalarProduct();
            var alpha = p.ScalarProduct(r) / pScalarProduct;

            solution.Sum(z.Multiply(alpha, firstBufferVector), solution);

            var rNext = r.Subtract(p.Multiply(alpha, firstBufferVector), r);

            _precondition.BackSubstitution(rNext, firstBufferVector);

            var LAUr = _precondition.ForwardElimination(matrix.Multiply(firstBufferVector, secondBufferVector), firstBufferVector);

            var beta = -(p.ScalarProduct(LAUr) / pScalarProduct);

            var zNext = _precondition.BackSubstitution(rNext, secondBufferVector).Sum(z.Multiply(beta, z), z);
            var pNext = LAUr.Sum(LAUr, p.Multiply(beta, p), p);

            r = rNext;
            z = zNext;
            p = pNext;
            residualNext = r.ScalarProduct() / residual;

            //CourseHolder.GetResidualInfo(i, residualNext);
        }

        //Console.WriteLine();

        return solution;
    }
}