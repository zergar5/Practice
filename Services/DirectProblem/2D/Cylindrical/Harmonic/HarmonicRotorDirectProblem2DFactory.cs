using Application.EquationSystems;
using Application.EquationSystems.MatrixDecompositions.LU;
using Application.EquationSystems.Preconditions.Separate;
using Application.EquationSystems.Solvers.Sparse;
using Application.FEM._1D.Assembling.Local;
using Application.FEM._2D;
using Application.FEM._2D.Assembling.Boundaries;
using Application.FEM._2D.Assembling.Global;
using Application.FEM._2D.Assembling.Global.Sources;
using Application.FEM._2D.Assembling.Local;
using Application.FEM._2D.BasisFunctions;
using Application.FEM._2D.Grid;
using Application.FEM.Assembling._2D.Boundaries.First;
using Application.FEM.Assembling.PortraitBuilders;
using Application.FEM.Core.Assembling.Boundaries.First;
using Application.FEM.Core.Assembling.Inserters;
using Application.FEM.Core.Grid.Splitting;
using Domain.Materials;
using Domain.Nodes;
using System.Numerics;

namespace Application.DirectProblem._2D.Cylindrical.Harmonic;

public class HarmonicRotorDirectProblem2DFactory : IDirectProblem2DFactory<Complex, GridBuilder2D.Grid2DParameters, MaterialWithSigmaMu>
{
    public IHarmonicDirectProblem2DWithSources<Complex, GridBuilder2D.Grid2DParameters, MaterialWithSigmaMu> Create()
    {
        var directProblemContextProvider = new HarmonicRotorDirectProblem2DContextProvider();
        var bilinearBasisFunctionsProvider = new BilinearBasisFunctionsProvider(directProblemContextProvider);

        var cylindricalLocalRotorStiffnessMatrixAssembler1D = new RotorLocalMatrixAssembler1D();
        var cylindricalLocalStiffnessMatrixAssembler1D = new CylindricalLocalStiffnessMatrixAssembler1D();
        var localStiffnessMatrixAssembler1D = new LocalStiffnessMatrixAssembler1D();
        var cylindricalLocalMassMatrixAssembler1D = new CylindricalLocalMassMatrixAssembler1D();
        var localMassMatrixAssembler1D = new LocalMassMatrixAssembler1D();

        var cylindricalLocalMassMatrixAssembler =
            new CylindricalLocalMassMatrixAssembler2D(cylindricalLocalMassMatrixAssembler1D, localMassMatrixAssembler1D);

        var cylindricalLocalStiffnessMatrixAssembler = new RotorLocalStiffnessMatrixAssembler2D
        (
            cylindricalLocalRotorStiffnessMatrixAssembler1D,
            cylindricalLocalStiffnessMatrixAssembler1D,
            localStiffnessMatrixAssembler1D,
            cylindricalLocalMassMatrixAssembler1D,
            localMassMatrixAssembler1D
        );

        var cylindricalLocalMatricesAssembler = new RotorLocalMatricesAssembler2D(cylindricalLocalStiffnessMatrixAssembler, cylindricalLocalMassMatrixAssembler);
        var localMatrixAssembler = new HarmonicRotorLocalMatrixAssembler2D(directProblemContextProvider, cylindricalLocalMatricesAssembler);

        var sparseInserter = new SparseInserter();

        var slaeSolver = new LocalOptimalScheme(new LUPrecondition(new LUIncompleteDecomposition()), new IterativeMethodConfig());

        return new HarmonicRotorDirectProblem2D
        (
            directProblemContextProvider,
            new GridBuilder2D(new AxisSplitter()),
            bilinearBasisFunctionsProvider,
            localMatrixAssembler,
            new DefinedValueFirstBoundaryResolver2D<Complex>(new BoundCoverageResolver2D(directProblemContextProvider)),
            new ProblemWithSourcesEquationAssembler2D<Complex>
            (
                new HarmonicMatrixPortraitBuilder<Node2D, IElement2D>(),
                sparseInserter,
                new ComplexFirstBoundaryApplier(),
                new HarmonicRotorSparseMatrixSourceApplier2D(directProblemContextProvider, sparseInserter)
            ),
            slaeSolver
        );
    }

    public IHarmonicDirectProblem2DWithSources<Complex, GridBuilder2D.Grid2DParameters, MaterialWithSigmaMu>[] Create(int count)
    {
        var directProblems =
            new IHarmonicDirectProblem2DWithSources<Complex, GridBuilder2D.Grid2DParameters, MaterialWithSigmaMu>[count];

        for (var i = 0; i < count; i++)
        {
            directProblems[i] = Create();
        }

        return directProblems;
    }
}