using System.Buffers;
using Application.DirectProblem;
using Application.DirectProblem._2D;
using Application.DirectProblem._2D.Cylindrical.Harmonic;
using Application.FEM.Core.Assembling.Local;
using Application.FEM.Core.Grid;
using Application.MathObjects.Matrices;
using Domain.Materials;
using Domain.Nodes;

namespace Application.FEM._2D.Assembling.Local;

public class HarmonicRotorLocalMatrixAssembler2D : ILocalMatrixAssembler<IElement2D, double>
{
    private readonly IDirectProblemContextProvider<HarmonicRotorDirectProblem2DContext> _problemContextProvider;
    private readonly ICylindricalLocalMatricesAssembler<IElement2D, double> _cylindricalLocalMatricesAssembler;

    public HarmonicRotorLocalMatrixAssembler2D
    (
        IDirectProblemContextProvider<HarmonicRotorDirectProblem2DContext> problemContextProvider,
        ICylindricalLocalMatricesAssembler<IElement2D, double> cylindricalLocalMatricesAssembler
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

        var matrix = MatrixPool<double>.Rent(element.NodeIndexes.Length * 2);
            
        var r = grid.Nodes[element.NodeIndexes[0]].R();
        var mass = _cylindricalLocalMatricesAssembler.AssembleMassMatrix(element, r);
        var stiffness = _cylindricalLocalMatricesAssembler.AssembleStiffnessMatrix(element, r);

        stiffness.Multiply(1d / MaterialWithSigmaMu.Mu, stiffness);

        for (var i = 0; i < element.NodeIndexes.Length; i++)
        {
            for (var j = 0; j < element.NodeIndexes.Length; j++)
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
        var complexIndexes = ArrayPool<int>.Shared.Rent(element.NodeIndexes.Length * 2);

        for (var i = 0; i < element.NodeIndexes.Length; i++)
        {
            complexIndexes[i * 2] = 2 * element.NodeIndexes[i];
            complexIndexes[i * 2 + 1] = complexIndexes[i * 2] + 1;
        }

        return complexIndexes;
    }
}