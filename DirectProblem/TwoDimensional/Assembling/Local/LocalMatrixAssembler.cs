﻿using DirectProblem.Core;
using DirectProblem.Core.Base;
using DirectProblem.Core.GridComponents;
using DirectProblem.FEM.Assembling.Local;
using DirectProblem.TwoDimensional.Assembling.MatrixTemplates;

namespace DirectProblem.TwoDimensional.Assembling.Local;

public class LocalMatrixAssembler : ILocalMatrixAssembler
{
    private Grid<Node2D> _grid;
    private readonly Matrix _stiffnessTemplate;
    private readonly Matrix _massTemplate;
    private readonly Matrix _massRTemplate;
    private readonly Matrix _rotorMassTemplate;
    private readonly Matrix _stiffness;
    private readonly Matrix _rotorStiffness;
    private readonly Matrix _stiffnessR;
    private readonly Matrix _stiffnessZ;
    private readonly Matrix _mass;
    private readonly Matrix _massR;
    private readonly Matrix _massZ;

    public LocalMatrixAssembler(Grid<Node2D> grid)
    {
        _grid = grid;
        _stiffness = new Matrix(4);
        _stiffnessTemplate = StiffnessMatrixTemplatesProvider.StiffnessMatrix;
        _massTemplate = MassMatrixTemplateProvider.MassMatrix;
        _massRTemplate = MassMatrixTemplateProvider.MassRMatrix;
        _rotorMassTemplate = MassMatrixTemplateProvider.RotorMassMatrix;
        _stiffness = new Matrix(4);
        _rotorStiffness = new Matrix(2);
        _stiffnessR = new Matrix(2);
        _stiffnessZ = new Matrix(2);
        _mass = new Matrix(4);
        _massR = new Matrix(2);
        _massZ = new Matrix(2);
    }

    public LocalMatrixAssembler SetGrid(Grid<Node2D> grid)
    {
        _grid = grid;

        return this;
    }

    public Matrix AssembleStiffnessMatrix(Element element)
    {
        var rotorStiffness = AssembleRotorStiffness(element);

        var stiffnessR = AssembleStiffnessR(element);
        var stiffnessZ = AssembleStiffnessZ(element);

        var massR = AssembleMassR(element);
        var massZ = AssembleMassZ(element);

        for (var i = 0; i < element.NodesIndexes.Length; i++)
        {
            for (var j = 0; j <= i; j++)
            {
                _stiffness[i, j] = stiffnessR[GetMuIndex(i), GetMuIndex(j)] * massZ[GetNuIndex(i), GetNuIndex(j)] +
                                   massR[GetMuIndex(i), GetMuIndex(j)] * stiffnessZ[GetNuIndex(i), GetNuIndex(j)] +
                                   rotorStiffness[GetMuIndex(i), GetMuIndex(j)] * massZ[GetNuIndex(i), GetNuIndex(j)];
                _stiffness[j, i] = _stiffness[i, j];
            }
        }

        return _stiffness;
    }

    public Matrix AssembleMassMatrix(Element element)
    {
        var massR = AssembleMassR(element);
        var massZ = AssembleMassZ(element);

        for (var i = 0; i < element.NodesIndexes.Length; i++)
        {
            for (var j = 0; j <= i; j++)
            {
                _mass[i, j] = massR[GetMuIndex(i), GetMuIndex(j)] * massZ[GetNuIndex(i), GetNuIndex(j)];
                _mass[j, i] = _mass[i, j];
            }
        }

        return _mass;
    }

    private Matrix AssembleStiffnessR(Element element)
    {
        Matrix.Multiply((2 * _grid.Nodes[element.NodesIndexes[0]].R + element.Length) / (2 * element.Length),
            _stiffnessTemplate, _stiffnessR);

        return _stiffnessR;
    }

    private Matrix AssembleStiffnessZ(Element element)
    {
        Matrix.Multiply(1d / element.Height,
            _stiffnessTemplate, _stiffnessZ);

        return _stiffnessZ;
    }

    private Matrix AssembleRotorStiffness(Element element)
    {
        var d = _grid.Nodes[element.NodesIndexes[0]].R / element.Length;

        _rotorStiffness[0, 0] = Math.Pow(1 + d, 2);
        _rotorStiffness[0, 1] = -d * (1 + d);
        _rotorStiffness[1, 0] = _rotorStiffness[0, 1];
        _rotorStiffness[1, 1] = Math.Pow(d, 2);

        Matrix.Multiply(Math.Log(1 + 1 / d), _rotorStiffness, _rotorStiffness);
        Matrix.Multiply(-d, _stiffnessTemplate, _stiffnessZ);
        Matrix.Multiply(0.5, _rotorMassTemplate, _massR);

        Matrix.Sum(_rotorStiffness, _stiffnessZ, _rotorStiffness);
        Matrix.Sum(_rotorStiffness, _massR, _rotorStiffness);

        return _rotorStiffness;
    }

    private Matrix AssembleMassR(Element element)
    {
        Matrix.Multiply(Math.Pow(element.Length, 2) / 12d,
            _massRTemplate, _massR);

        Matrix.Multiply(element.Length * _grid.Nodes[element.NodesIndexes[0]].R / 6d,
            _massTemplate, _massZ);

        Matrix.Sum(_massR, _massZ, _massR);

        return _massR;
    }

    private Matrix AssembleMassZ(Element element)
    {
        Matrix.Multiply(element.Height / 6d,
            _massTemplate, _massZ);

        return _massZ;
    }

    private int GetMuIndex(int i)
    {
        return i % 2;
    }

    private int GetNuIndex(int i)
    {
        return i / 2;
    }
}