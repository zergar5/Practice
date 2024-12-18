using System.Numerics;
using DirectProblem.Core;
using DirectProblem.Core.GridComponents;
using DirectProblem.FEM.Assembling.Local;

namespace DirectProblem.TwoDimensional.Assembling.Local;

public class DenseValuesProviderViaFunction : IDenseValuesProvider<Complex>
{
    private readonly Func<Node2D, Complex> _function;
    private readonly Grid<Node2D> _grid;

    public DenseValuesProviderViaFunction(Func<Node2D, Complex> function, Grid<Node2D> grid)
    {
        _function = function;
        _grid = grid;
    }

    public Complex[] GetValues(Element element)
    {
        var values = new Complex[element.NodesIndexes.Length];

        for (var i = 0; i < element.NodesIndexes.Length; i++)
        {
            values[i] = _function(_grid.Nodes[element.NodesIndexes[i]]);
        }

        return values;
    }
}