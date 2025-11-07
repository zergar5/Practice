using System.Buffers;
using Application.DirectProblem._2D;
using Application.FEM.Core.BasisFunctions;
using Application.FEM.Core.Grid;
using Application.MathObjects.Vectors;
using Domain.Nodes;
using System.Numerics;

namespace Application.FEM._2D;

public class ComplexSolutionResolver2D : ISolutionResolver2D<Complex>
{
    private readonly IGrid<Node2D, IElement2D> _grid;
    private readonly IVector<double> _solution;
    private readonly IBasisFunctionsProvider<Node2D, double, IElement2D> _basisFunctionsProvider;

    public ComplexSolutionResolver2D
    (
        IGrid<Node2D, IElement2D> grid,
        IVector<double> solution,
        IBasisFunctionsProvider<Node2D, double, IElement2D> basisFunctionsProvider
    )
    {
        _grid = grid;
        _solution = solution;
        _basisFunctionsProvider = basisFunctionsProvider;
    }

    public Complex Get(Node2D node)
    {
        if (_grid.Has(node))
        {
            var element = _grid.FindNodeElement(node);

            if (element == null)
            {
                return new Complex(double.NaN, double.NaN);
            }

            var sumS = 0d;
            var sumC = 0d;
            var basisFunctions = _basisFunctionsProvider.GetFunctions(element);

            for (var i = 0; i < element.NodeIndexes.Length; i++)
            {
                sumS += _solution[element.NodeIndexes[i] * 2] * basisFunctions[i].Evaluate(node);
                sumC += _solution[element.NodeIndexes[i] * 2 + 1] * basisFunctions[i].Evaluate(node);
            }

            var values = new Complex(sumS, sumC);

            ArrayPool<IBasisFunction<Node2D, double>>.Shared.Return(basisFunctions);

            //CourseHolder.WriteSolution(point, values);

            return values;
        }

        //CourseHolder.WriteAreaInfo();
        //CourseHolder.WriteSolution(node, (double.NaN, double.NaN));
        return new Complex(double.NaN, double.NaN);
    }
}