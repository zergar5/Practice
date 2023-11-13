using DirectProblem.Core;
using DirectProblem.Core.Base;
using DirectProblem.Core.GridComponents;
using DirectProblem.FEM;
using DirectProblem.GridGenerator.Intervals.Core;
using DirectProblem.TwoDimensional.Assembling.Local;
using System.Drawing;
using System.Numerics;
using Vector = DirectProblem.Core.Base.Vector;

namespace DirectProblem.TwoDimensional;

public class FEMSolution
{
    private readonly Grid<Node2D> _grid;
    private readonly Vector _solution;
    private readonly LocalBasisFunctionsProvider _localBasisFunctionsProvider;
    private readonly double _omega;

    public FEMSolution
    (
        Grid<Node2D> grid,
        Vector solution,
        LocalBasisFunctionsProvider localBasisFunctionsProvider,
        double omega
    )
    {
        _grid = grid;
        _solution = solution;
        _localBasisFunctionsProvider = localBasisFunctionsProvider;
        _omega = omega;
    }

    public Complex Calculate(Node2D point)
    {
        if (AreaHas(point))
        {
            var element = _grid.Elements.First(x => ElementHas(x, point));

            var basisFunctions = _localBasisFunctionsProvider.GetBilinearFunctions(element);

            var sumS = 0d;
            var sumC = 0d;

            for (var i = 0; i < element.NodesIndexes.Length; i++)
            {
                sumS += _solution[element.NodesIndexes[i] * 2] * basisFunctions[i].Calculate(point);
                sumC += _solution[element.NodesIndexes[i] * 2 + 1] * basisFunctions[i].Calculate(point);
            }

            var values = new Complex(sumS, sumC);

            //CourseHolder.WriteSolution(point, values);

            return values;
        }

        CourseHolder.WriteAreaInfo();
        CourseHolder.WriteSolution(point, (double.NaN, double.NaN));
        return new Complex(double.NaN, double.NaN);
    }

    public Complex CalculateEMF(Node2D point)
    {
        var emfs = 2d * Math.PI * point.R * Calculate(point);

        return emfs;
    }

    public Complex[] CalculateEMFs(Node2D[] points)
    {
        var emfsValues = new Complex[points.Length];

        for (var i = 0; i < points.Length; i++)
        {
            emfsValues[i] = CalculateEMF(points[i]);
        }

        return emfsValues;
    }

    public double CalcError(Func<Node2D, Complex> u)
    {
        var trueSolution = new Vector(_solution.Count);

        for (var i = 0; i < trueSolution.Count / 2; i++)
        {
            var uValues = u(_grid.Nodes[i]);
            trueSolution[i * 2] = uValues.Real;
            trueSolution[i * 2 + 1] = uValues.Imaginary;
        }

        Vector.Subtract(_solution, trueSolution, trueSolution);

        return trueSolution.Norm;
    }

    private bool ElementHas(Element element, Node2D node)
    {
        var leftCornerNode = _grid.Nodes[element.NodesIndexes[0]];
        var rightCornerNode = _grid.Nodes[element.NodesIndexes[^1]];
        return node.R >= leftCornerNode.R && node.Z >= leftCornerNode.Z &&
               node.R <= rightCornerNode.R && node.Z <= rightCornerNode.Z;
    }

    private bool AreaHas(Node2D node)
    {
        var leftCornerNode = _grid.Nodes[0];
        var rightCornerNode = _grid.Nodes[^1];
        return node.R >= leftCornerNode.R && node.Z >= leftCornerNode.Z &&
               node.R <= rightCornerNode.R && node.Z <= rightCornerNode.Z;
    }
}