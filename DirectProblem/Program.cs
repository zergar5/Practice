using DirectProblem.Calculus;
using DirectProblem.Core.GridComponents;
using DirectProblem.FEM;
using DirectProblem.GridGenerator;
using DirectProblem.GridGenerator.Intervals.Splitting;
using DirectProblem.SLAE.Preconditions;
using DirectProblem.SLAE.Solvers;
using DirectProblem.TwoDimensional;
using DirectProblem.TwoDimensional.Assembling;
using DirectProblem.TwoDimensional.Assembling.Boundary;
using DirectProblem.TwoDimensional.Assembling.Global;
using DirectProblem.TwoDimensional.Assembling.Local;
using DirectProblem.TwoDimensional.Parameters;
using System.Globalization;
using System.Numerics;
using DirectProblem.TwoDimensional.Assembling.MatrixTemplates;
using Vector = DirectProblem.Core.Base.Vector;

Thread.CurrentThread.CurrentCulture = CultureInfo.InvariantCulture;

var gridBuilder2D = new GridBuilder2D();
var grid = gridBuilder2D
    .SetRAxis(new AxisSplitParameter(
            new[] { 1e-3d, 0.101d, 1d },
            new UniformSplitter(2),
            new UniformSplitter(1),
            new UniformSplitter(7)
        )
    )
    .SetZAxis(new AxisSplitParameter(
            new[] { -12d, -9d, -6d, -3d, 1e-3d },
            new ProportionalSplitter(4, 0.7),
            new ProportionalSplitter(16, 0.9),
            new ProportionalSplitter(16, 1.1),
            new ProportionalSplitter(4, 1.3)
        )
    )
    //подумать как задавать области
    .SetAreas(new Area[]
    {
        new(0, new Node2D(0d, 0d), new Node2D(1d, 1d)),
        new(1, new Node2D(1d, 0d), new Node2D(2d, 1d)),
        new(2, new Node2D(0d, 1d), new Node2D(1d, 2d)),
        new(3, new Node2D(1d, 1d), new Node2D(2d, 2d))
    })
    .Build();

var materialFactory = new MaterialFactory
(
    new List<double> { 1d, 1d, 1d, 1d, 1d, 1d, 1d, 1d, 1d },
    //подставить другие сигма
    new List<double> { 1d, 1d, 1d, 1d, 1d, 1d, 1d, 1d, 1d }
);

var omega = 40000d;

var localBasisFunctionsProvider = new LocalBasisFunctionsProvider(grid, new LinearFunctionsProvider());

var localAssembler =
    new LocalAssembler(
        new LocalMatrixAssembler(grid, new StiffnessMatrixTemplatesProvider(), new MassMatrixTemplateProvider()),
        materialFactory, omega);

var inserter = new Inserter();
var globalAssembler = new GlobalAssembler<Node2D>(grid, new MatrixPortraitBuilder(), localAssembler, inserter, new GaussExcluder(), localBasisFunctionsProvider);

var firstBoundaryProvider = new FirstBoundaryProvider(grid);
var conditions = firstBoundaryProvider.GetConditions(2, 2);

var equation = globalAssembler
    .AssembleEquation(grid)
    .ApplySources(new FocusedSource[] {new(new Node2D(0.0505d, -1d), 1)})
    .ApplyFirstConditions(conditions)
    .BuildEquation();

var preconditionMatrix = globalAssembler.AllocatePreconditionMatrix();

var luPreconditioner = new LUPreconditioner();

var los = new LOS(luPreconditioner, new LUSparse(), preconditionMatrix);
var solution = los.Solve(equation);

var femSolution = new FEMSolution(grid, solution, localBasisFunctionsProvider, omega);

var emfsValues = femSolution.CalculateEMFs(new Receiver[] {new(new Node2D(0.0505d, -2d))});