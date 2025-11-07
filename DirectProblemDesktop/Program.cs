using Application.DirectProblem;
using Application.DirectProblem._2D;
using Application.DirectProblem._2D.Cylindrical.Harmonic;
using Application.EquationSystems;
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
using Application.FEM.Assembling._1D;
using Application.FEM.Assembling._2D.Boundaries.First;
using Application.FEM.Assembling.PortraitBuilders;
using Application.FEM.Core.Assembling.Boundaries.First;
using Application.FEM.Core.Assembling.Inserters;
using Application.FEM.Core.Grid.Splitting;
using DirectProblemDesktop;
using Domain.Boundaries;
using Domain.Edges;
using Domain.Environment;
using Domain.Nodes;
using System.Diagnostics;
using System.Globalization;
using System.Numerics;
using Application.EquationSystems.MatrixDecompositions.LU;
using Domain.Enums;

Thread.CurrentThread.CurrentCulture = CultureInfo.InvariantCulture;

var gridParameters = TestGridParameters.GetUniformGridWith0Dot003125Step();
var materials = TestMaterials.GetMaterialsForUniformGridWith0Dot003125Step();

var frequencies = new[] { 4e4, 2e5, 1e6, 2e6 };
const int sourcePower = 1;

var sources = new Source<Node2D>[10];
var receiverLines = new ReceiverLine<Node2D>[sources.Length];

var emfs = new Complex[sources.Length, frequencies.Length];
var phaseDifferences = new double[sources.Length, frequencies.Length];
var centersZ = new double[sources.Length];

for (var i = 0; i < sources.Length; i++)
{
    sources[i] = new Source<Node2D>
    {
        Location = new Node2D
        {
            X = 0.05,
            Y = -2.5 - 0.1 * i
        },
        Power = sourcePower,
    };

    receiverLines[i] = new ReceiverLine<Node2D>
    {
        ReceiverM = new Node2D
        {
            X = sources[i].Location.R(),
            Y = sources[i].Location.Z() - 0.05,
        },
        ReceiverN = new Node2D
        {
            X = sources[i].Location.R(),
            Y = sources[i].Location.Z() - 0.1,
        }
    };

    centersZ[i] = (sources[i].Location.Z() + receiverLines[i].ReceiverN.Z()) / 2;
}

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

var slaeSolver = new LocalOptimalScheme<double>(new LUPrecondition<double>(new LUIncompleteDecomposition<double>()), new IterativeMethodConfig());

var directProblem = new HarmonicRotorDirectProblem2D
(
    directProblemContextProvider,
    new GridBuilder2D(new AxisSplitter()),
    bilinearBasisFunctionsProvider,
    localMatrixAssembler,
    new DefinedValueFirstBoundaryResolver2D<Complex>(new BoundCoverageResolver2D(directProblemContextProvider)),
    new ProblemWithSourcesEquationAssembler2D<double, Complex>
    (
        new HarmonicMatrixPortraitBuilder<Node2D, IElement2D>(),
        new SparseInserter<double>(),
        new ComplexFirstBoundaryApplier(),
        new HarmonicRotorSparseMatrixSourceApplier2D(directProblemContextProvider, new SparseInserter<double>())
    ),
    slaeSolver
);

directProblem.GenerateGrid(gridParameters);
directProblem.SetMaterials(materials);

var firstBoundaries = new IDefinedValueBoundaryCondition<Edge<Node2D>, Complex>[]
{
    new EdgeBound<Node2D, Complex>
    {
        Attachment = new Edge<Node2D>
        {
            BeginNode = new Node2D
            {
                X = gridParameters.XControlPoints[0],
                Y = gridParameters.YControlPoints[0]
            },
            EndNode = new Node2D
            {
                X = gridParameters.XControlPoints[^1],
                Y = gridParameters.YControlPoints[0]
            }
        },
        Value = new Complex(0, 0),
        Type = BoundaryConditionType.First
    },
    new EdgeBound<Node2D, Complex>
    {
        Attachment = new Edge<Node2D>
        {
            BeginNode = new Node2D
            {
                X = gridParameters.XControlPoints[0],
                Y = gridParameters.YControlPoints[0]
            },
            EndNode = new Node2D
            {
                X = gridParameters.XControlPoints[0],
                Y = gridParameters.YControlPoints[^1]
            }
        },
        Value = new Complex(0, 0),
        Type = BoundaryConditionType.First
    },
    new EdgeBound<Node2D, Complex>
    {
        Attachment = new Edge<Node2D>
        {
            BeginNode = new Node2D
            {
                X = gridParameters.XControlPoints[^1],
                Y = gridParameters.YControlPoints[0]
            },
            EndNode = new Node2D
            {
                X = gridParameters.XControlPoints[^1],
                Y = gridParameters.YControlPoints[^1]
            }
        },
        Value = new Complex(0, 0),
        Type = BoundaryConditionType.First
    },
    new EdgeBound<Node2D, Complex>
    {
        Attachment = new Edge<Node2D>
        {
            BeginNode = new Node2D
            {
                X = gridParameters.XControlPoints[0],
                Y = gridParameters.YControlPoints[^1]
            },
            EndNode = new Node2D
            {
                X = gridParameters.XControlPoints[^1],
                Y = gridParameters.YControlPoints[^1]
            }
        },
        Value = new Complex(0, 0),
        Type = BoundaryConditionType.First
    }
};

directProblem.SetDefinedValueBoundaryCondition(firstBoundaries);

var stopwatch = new Stopwatch();
stopwatch.Start();

for (var i = 0; i < sources.Length; i++)
{
    for (var j = 0; j < frequencies.Length; j++)
    {
        directProblem.SetFrequency(frequencies[j]);
        directProblem.SetSources([sources[j]]);

        var solution = directProblem.Solve();

        var potentialM = solution.Get(receiverLines[i].ReceiverM);
        var potentialN = solution.Get(receiverLines[i].ReceiverN);

        emfs[i, j] = 2 * Math.PI * receiverLines[i].ReceiverM.R() * potentialM;

        phaseDifferences[i, j] = (potentialM.Phase - potentialN.Phase) * 180d / Math.PI;

        Console.Write($"source {i} frequency {j}                                   \r");
    }
}

stopwatch.Stop();

var time = (double)stopwatch.ElapsedMilliseconds / 1000;

Console.WriteLine();
Console.WriteLine($"Elapsed time {time}");