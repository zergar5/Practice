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

    public LOS SetPrecondition(SparseMatrix preconditionMatrix)
    {
        _preconditionMatrix = preconditionMatrix;
        return this;
    }

    private void PrepareProcess(Equation<SparseMatrix> equation)
    {
        _luPreconditioner.Decompose(_preconditionMatrix);
        _r = SparseMatrix.ParallelMultiply(equation.Matrix, equation.Solution, _r);
        _luSparse.CalcY(_preconditionMatrix, Vector.ParallelSubtract(equation.RightPart, _r, _r), _r);
        _z = _luSparse.CalcX(_preconditionMatrix, _r, _z);
        _p = SparseMatrix.ParallelMultiply(equation.Matrix, _z, _p);
        _luSparse.CalcY(_preconditionMatrix, _p, _p);
    }

    public Vector Solve(Equation<SparseMatrix> equation)
    {
        PrepareProcess(equation);
        IterationProcess(equation);
        return equation.Solution;
    }

    private void IterationProcess(Equation<SparseMatrix> equation)
    {
        //Console.WriteLine("LOS");

        var residual = Vector.ScalarProduct(_r, _r);
        var residualNext = residual;
        var bufferVector = new Vector(equation.Solution.Count);

        for (var i = 1; i <= MethodsConfig.MaxIterations && residualNext > Math.Pow(MethodsConfig.MethodPrecision, 2); i++)
        {
            var scalarPP = Vector.ScalarProduct(_p, _p);
            var alpha = Vector.ScalarProduct(_p, _r) / scalarPP;

            Vector.ParallelMultiply(alpha, _z, bufferVector);
            Vector.ParallelSum(equation.Solution, bufferVector, equation.Solution);

            var rNext = Vector.ParallelSubtract(_r, Vector.ParallelMultiply(alpha, _p, bufferVector), _r);

            _luSparse.CalcX(_preconditionMatrix, rNext, bufferVector);

            var LAUr = _luSparse.CalcY(_preconditionMatrix, SparseMatrix.ParallelMultiply(equation.Matrix, bufferVector, equation.RightPart), bufferVector);

            var beta = -(Vector.ScalarProduct(_p, LAUr) / scalarPP);

            var zNext = Vector.ParallelSum(_luSparse.CalcX(_preconditionMatrix, rNext, equation.RightPart), Vector.ParallelMultiply(beta, _z, _z), _z);
            var pNext = Vector.ParallelSum(LAUr, Vector.ParallelMultiply(beta, _p, _p), _p);

            _r = rNext;
            _z = zNext;
            _p = pNext;

            residualNext = Vector.ScalarProduct(_r, _r) / residual;

            //CourseHolder.GetInfo(i, residualNext);
        }

        //Console.WriteLine();
    }
}