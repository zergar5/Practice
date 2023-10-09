using DirectProblem.Core.Base;
using DirectProblem.Core.Global;
using DirectProblem.FEM;
using DirectProblem.SLAE.Preconditions;

namespace DirectProblem.SLAE.Solvers;

public class LOS
{
    private readonly LUPreconditioner _luPreconditioner;
    private readonly LUSparse _luSparse;
    private SparseMatrix _preconditionMatrix;
    private Vector _r;
    private Vector _z;
    private Vector _p;

    public LOS(LUPreconditioner luPreconditioner, LUSparse luSparse)
    {
        _luPreconditioner = luPreconditioner;
        _luSparse = luSparse;
    }

    private void PrepareProcess(Equation<SparseMatrix> equation)
    {
        _preconditionMatrix = _luPreconditioner.Decompose(_preconditionMatrix);
        SparseMatrix.Multiply(equation.Matrix, equation.Solution, _r);
        _luSparse.CalcY(_preconditionMatrix, Vector.Subtract(equation.RightPart, _r, _r), _r);
        _luSparse.CalcX(_preconditionMatrix, _r, _z);
        _luSparse.CalcY(_preconditionMatrix, SparseMatrix.Multiply(equation.Matrix, _z, _p), _p);
    }

    public Vector Solve(Equation<SparseMatrix> equation, SparseMatrix preconditionMatrix)
    {
        _preconditionMatrix = preconditionMatrix;
        PrepareProcess(equation);
        IterationProcess(equation);
        return equation.Solution;
    }

    private void IterationProcess(Equation<SparseMatrix> equation)
    {
        Console.WriteLine("LOS");

        var residual = Vector.ScalarProduct(_r, _r);
        var residualNext = residual;
        var bufferVector = new Vector(equation.Solution.Count);

        for (var i = 1; i <= MethodsConfig.MaxIterations && residualNext > Math.Pow(MethodsConfig.Eps, 2); i++)
        {
            var scalarPP = Vector.ScalarProduct(_p, _p);
            var alpha = Vector.ScalarProduct(_p, _r) / scalarPP;

            Vector.Multiply(alpha, _z, bufferVector);
            Vector.Sum(equation.Solution, bufferVector, equation.Solution);

            var rNext = Vector.Subtract(_r, Vector.Multiply(alpha, _p, bufferVector), _r);

            _luSparse.CalcX(_preconditionMatrix, rNext, bufferVector);

            var LAUr = _luSparse.CalcY(_preconditionMatrix, SparseMatrix.Multiply(equation.Matrix, bufferVector, equation.RightPart), bufferVector);

            var beta = -(Vector.ScalarProduct(_p, LAUr) / scalarPP);

            var zNext = Vector.Sum(_luSparse.CalcX(_preconditionMatrix, rNext, bufferVector), Vector.Multiply(beta, _z, _z), _z);
            var pNext = Vector.Sum(LAUr, Vector.Multiply(beta, _p, _p), _p);

            _r = rNext;
            _z = zNext;
            _p = pNext;

            residualNext = Vector.ScalarProduct(_r, _r) / residual;

            CourseHolder.GetInfo(i, residualNext);
        }

        Console.WriteLine();
    }
}