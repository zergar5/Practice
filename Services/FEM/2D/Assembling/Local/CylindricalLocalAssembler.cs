using Application.FEM.Core.Assembling.Local;
using Application.FEM.Core.Grid;
using Application.MathObjects.Matrices;
using Domain.Materials;
using Domain.Nodes;

namespace Application.FEM._2D.Assembling.Local;

public class HarmonicCylindricalLocalMatrixAssembler2D : ILocalMatrixAssembler<Element2D, double>
{
    private readonly ICylindricalLocalMatricesAssembler<Element2D, double> _cylindricalLocalMatricesAssembler;
    // TODO параметры ниже нужно доставать из текущего скопа или пройвадера
    private readonly Grid<Node2D> _grid;
    private readonly MaterialWithSigmaMu[] _materials;
    private readonly double _frequency;

    public HarmonicCylindricalLocalMatrixAssembler2D
    (
        ICylindricalLocalMatricesAssembler<Element2D, double> cylindricalLocalMatricesAssembler,
        Grid<Node2D> grid,
        MaterialWithSigmaMu[] materials,
        double frequency
    )
    {
        _cylindricalLocalMatricesAssembler = cylindricalLocalMatricesAssembler;
        _grid = grid;
        _materials = materials;
        _frequency = frequency;
    }

    public ILocalMatrix<double> AssembleMatrix(Element2D element)
    {
        var matrix = new Matrix<double>(element.NodeIndexes.Length * 2);
        var material = _materials[element.MaterialId];

        var r = _grid.Nodes[element.NodeIndexes[0]].R();
        var mass = _cylindricalLocalMatricesAssembler.AssembleMassMatrix(element, r);
        var stiffness = _cylindricalLocalMatricesAssembler.AssembleStiffnessMatrix(element, r);

        stiffness.Multiply(1d / material.Mu, stiffness);

        for (var i = 0; i < element.NodeIndexes.Length; i++)
        {
            for (var j = 0; j < element.NodeIndexes.Length; j++)
            {
                var massValue = _frequency * material.Sigma * mass[i, j];
                matrix[i * 2, j * 2] = stiffness[i, j];
                matrix[i * 2, j * 2 + 1] = -massValue;
                matrix[i * 2 + 1, j * 2] = massValue;
                matrix[i * 2 + 1, j * 2 + 1] = stiffness[i, j];
            }
        }

        var indexes = GetComplexIndexes(element);

        return new LocalMatrix<double>(matrix, indexes);
    }

    private int[] GetComplexIndexes(Element2D element)
    {
        var complexIndexes = new int[element.NodeIndexes.Length * 2];
        for (var i = 0; i < element.NodeIndexes.Length; i++)
        {
            complexIndexes[i * 2] = 2 * element.NodeIndexes[i];
            complexIndexes[i * 2 + 1] = complexIndexes[i * 2] + 1;
        }

        return complexIndexes;
    }
}