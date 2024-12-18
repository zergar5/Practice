using System.Numerics;
using DirectProblem.Core;
using DirectProblem.Core.GridComponents;
using DirectProblem.FEM.Assembling.Local;

namespace DirectProblem.TwoDimensional.Assembling.Local;

public class FieldPartDenseValuesProvider : IDenseValuesProvider<Complex>
{
    private readonly FEMSolution _solution;
    private readonly Grid<Node2D> _grid;
    private readonly int _materialId;
    private readonly double _delta;

    public FieldPartDenseValuesProvider(FEMSolution solution, Grid<Node2D> grid, int materialId, double delta)
    {
        _solution = solution;
        _grid = grid;
        _materialId = materialId;
        _delta = delta;
    }


    public Complex[] GetValues(Element element)
    {
        var values = new Complex[element.NodesIndexes.Length];

        if (element.MaterialId != _materialId) return values;

        for (var i = 0; i < element.NodesIndexes.Length; i++)
        {
            var value = _solution.Calculate(_grid.Nodes[element.NodesIndexes[i]]);
            values[i] = -_delta * new Complex(-value.Imaginary, value.Real);
        }

        return values;
    }
}