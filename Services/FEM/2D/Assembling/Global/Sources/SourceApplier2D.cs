using Application.DirectProblem;
using Application.DirectProblem._2D.Cylindrical.Harmonic;
using Application.FEM._1D.MatrixTemplates;
using Application.FEM.Core.Assembling.Global.Sources;
using Application.FEM.Core.Assembling.Inserters;
using Application.FEM.Core.Assembling.Local;
using Application.FEM.Core.Grid;
using Application.MathObjects.Equation;
using Application.MathObjects.Matrices;
using Application.MathObjects.Vectors;
using Domain.Enums;
using Domain.Environment;
using Domain.Nodes;

namespace Application.FEM._2D.Assembling.Global.Sources;

public interface ISourceApplier2D<in TMatrix> : ISourceApplier<TMatrix, Node2D>;

public class HarmonicRotorSparseMatrixSourceApplier2D : ISourceApplier2D<ISparseMatrix<double>>
{
    private readonly IDirectProblemContextProvider<HarmonicRotorDirectProblem2DContext> _problemContextProvider;
    private readonly ISparseInserter _inserter;

    private readonly IMatrix<int> _massMatrix;

    public HarmonicRotorSparseMatrixSourceApplier2D
    (
        IDirectProblemContextProvider<HarmonicRotorDirectProblem2DContext> problemContextProvider,
        ISparseInserter inserter
    )
    {
        _problemContextProvider = problemContextProvider;
        _inserter = inserter;
        _massMatrix = LagrangeMatrixTemplates.MassMatrix;
    }

    public void Apply(IEquation<ISparseMatrix<double>, double> equation, Source<Node2D> source)
    {
        var element = _problemContextProvider.Get().Grid.FindNodeElement(source.Location);

        if (element == null)
            throw new ArgumentException("Source location not in grid");

        var theta = source.Power / (2 * Math.PI * source.Location.R() * element.Height);

        var boundInfo = element.GetBoundNodeIds(Bound2D.Right);
        var thetaVector = VectorPool<double>.Rent(boundInfo.NodeIds.Length);

        for (var i = 0; i < thetaVector.Count; i++)
        {
            thetaVector[i] = theta;
        }

        var mass = _massMatrix.Multiply(element.Height * source.Location.R() / 6d, MatrixPool<double>.Rent(_massMatrix.RowCount));
        var sourceImpact = mass.Multiply(thetaVector, VectorPool<double>.Rent(thetaVector.Count));
        var complexIndexes = GetComplexIndexes(boundInfo.NodeIds);

        var complexVector = VectorPool<double>.Rent(thetaVector.Count * 2);

        for (var i = 0; i < boundInfo.NodeIds.Length; i++)
        {
            complexVector[i * 2] = sourceImpact[i];
            complexVector[i * 2 + 1] = 0d;
        }

        _inserter.InsertVector(equation.RightPart, new LocalVector<double>(complexVector, complexIndexes));

        VectorPool<double>.Return(thetaVector);
        MatrixPool<double>.Return(mass);
        VectorPool<double>.Return(sourceImpact);
        VectorPool<double>.Return(complexVector);
    }

    private int[] GetComplexIndexes(int[] indexes)
    {
        var complexIndexes = new int[2 * indexes.Length];

        for (var i = 0; i < indexes.Length; i++)
        {
            complexIndexes[i * 2] = 2 * indexes[i];
            complexIndexes[i * 2 + 1] = complexIndexes[i * 2] + 1;
        }

        return complexIndexes;
    }
}