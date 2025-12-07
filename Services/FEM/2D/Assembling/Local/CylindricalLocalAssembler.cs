using Application.DirectProblem;
using Application.DirectProblem._2D.Cylindrical.Harmonic;
using Application.FEM.Core.Assembling.Local;
using Application.MathObjects.Matrices;
using Domain.Materials;
using Domain.Nodes;
using System.Buffers;

namespace Application.FEM._2D.Assembling.Local;

public class HarmonicRotorLocalMatrixAssembler2D : ILocalMatrixAssembler<IElement2D>
{
    private readonly IDirectProblemContextProvider<HarmonicRotorDirectProblem2DContext> _problemContextProvider;
    private readonly ICylindricalLocalMatricesAssembler<IElement2D> _cylindricalLocalMatricesAssembler;

    public HarmonicRotorLocalMatrixAssembler2D
    (
        IDirectProblemContextProvider<HarmonicRotorDirectProblem2DContext> problemContextProvider,
        ICylindricalLocalMatricesAssembler<IElement2D> cylindricalLocalMatricesAssembler
    )
    {
        _problemContextProvider = problemContextProvider;
        _cylindricalLocalMatricesAssembler = cylindricalLocalMatricesAssembler;
    }

    public ILocalMatrix<double> AssembleMatrix(IElement2D element)
    {
        var problemContext = _problemContextProvider.Get();
        var grid = problemContext.Grid;
        var material = problemContext.Materials[element.MaterialId];
        var frequency = problemContext.Frequency;

        var matrix = MatrixPool<double>.Rent(element.NodeIndexes.Count * 2);

        var r = grid.Nodes[element.NodeIndexes[0]].R();
        var mass = _cylindricalLocalMatricesAssembler.AssembleMassMatrix(element, r);
        var stiffness = _cylindricalLocalMatricesAssembler.AssembleStiffnessMatrix(element, r);

        stiffness.Multiply(1d / MaterialWithSigmaMu.Mu, stiffness);

        for (var i = 0; i < element.NodeIndexes.Count; i++)
        {
            for (var j = 0; j < element.NodeIndexes.Count; j++)
            {
                var massValue = frequency * material.Sigma * mass[i, j];
                matrix[i * 2, j * 2] = stiffness[i, j];
                matrix[i * 2, j * 2 + 1] = -massValue;
                matrix[i * 2 + 1, j * 2] = massValue;
                matrix[i * 2 + 1, j * 2 + 1] = stiffness[i, j];
            }
        }

        var indexes = GetComplexIndexes(element);

        MatrixPool<double>.Return(mass);
        MatrixPool<double>.Return(stiffness);

        return new LocalMatrix<double>(matrix, indexes);
    }

    private static int[] GetComplexIndexes(IElement2D element)
    {
        var complexIndexes = ArrayPool<int>.Shared.Rent(element.NodeIndexes.Count * 2);

        for (var i = 0; i < element.NodeIndexes.Count; i++)
        {
            complexIndexes[i * 2] = 2 * element.NodeIndexes[i];
            complexIndexes[i * 2 + 1] = complexIndexes[i * 2] + 1;
        }

        return complexIndexes;
    }
}