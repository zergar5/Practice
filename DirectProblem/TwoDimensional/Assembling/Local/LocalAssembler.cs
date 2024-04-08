using DirectProblem.Core.Base;
using DirectProblem.Core.GridComponents;
using DirectProblem.Core.Local;
using DirectProblem.FEM.Assembling.Local;

namespace DirectProblem.TwoDimensional.Assembling.Local;

public class LocalAssembler : ILocalAssembler
{
    private readonly ILocalMatrixAssembler _localMatrixAssembler;
    private Material[] _materials;
    private double _frequency;
    private readonly Matrix _matrix = new(8);
    private readonly int[] _complexIndexes = new int[8];

    public LocalAssembler
    (
        ILocalMatrixAssembler localMatrixAssembler,
        Material[] materials
    )
    {
        _localMatrixAssembler = localMatrixAssembler;
        _materials = materials;
    }

    public LocalAssembler SetMaterials(Material[] materials)
    {
        _materials = materials;

        return this;
    }

    public LocalAssembler SetFrequency(double frequency)
    {
        _frequency = frequency;

        return this;
    }

    public LocalMatrix AssembleMatrix(Element element)
    {
        var matrix = GetComplexMatrix(element);
        var indexes = GetComplexIndexes(element);

        return new LocalMatrix(indexes, matrix);
    }

    private Matrix GetStiffnessMatrix(Element element)
    {
        var stiffness = _localMatrixAssembler.AssembleStiffnessMatrix(element);

        return stiffness;
    }

    private Matrix GetMassMatrix(Element element)
    {
        var mass = _localMatrixAssembler.AssembleMassMatrix(element);

        return mass;
    }

    private Matrix GetComplexMatrix(Element element)
    {
        var mass = GetMassMatrix(element);
        var stiffness = GetStiffnessMatrix(element);
        var material = _materials[element.MaterialId];

        Matrix.Multiply(1d / material.Mu, stiffness, stiffness);

        for (var i = 0; i < element.NodesIndexes.Length; i++)
        {
            for (var j = 0; j < element.NodesIndexes.Length; j++)
            {
                var massValue = _frequency * material.Sigma * mass[i, j];
                _matrix[i * 2, j * 2] = stiffness[i, j];
                _matrix[i * 2, j * 2 + 1] = -massValue;
                _matrix[i * 2 + 1, j * 2] = massValue;
                _matrix[i * 2 + 1, j * 2 + 1] = stiffness[i, j];
            }
        }

        return _matrix;
    }

    private int[] GetComplexIndexes(Element element)
    {
        for (var i = 0; i < element.NodesIndexes.Length; i++)
        {
            _complexIndexes[i * 2] = 2 * element.NodesIndexes[i];
            _complexIndexes[i * 2 + 1] = _complexIndexes[i * 2] + 1;
        }

        return _complexIndexes;
    }
}