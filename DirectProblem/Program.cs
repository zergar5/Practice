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
using DirectProblem;
using DirectProblem.IO;
using DirectProblem.TwoDimensional.Assembling.MatrixTemplates;
using Vector = DirectProblem.Core.Base.Vector;

Thread.CurrentThread.CurrentCulture = CultureInfo.InvariantCulture;

var gridBuilder2D = new GridBuilder2D();
var grid = gridBuilder2D
   .SetRAxis(new AxisSplitParameter(
           new[] { 1e-15, 0.1, 1 },
           new UniformSplitter(2),
           new UniformSplitter(18)
       )
   )
   .SetZAxis(new AxisSplitParameter(
           new[] { -12d, -9d, -6d, -3d, 0d },
           new ProportionalSplitter(4, 0.7),
           new ProportionalSplitter(16, 0.9),
           new ProportionalSplitter(16, 1.1),
           new ProportionalSplitter(4, 1.3)
       )
   )
   .SetAreas(new Area[]
   {
       //скважина
       new(0, new Node2D(1e-15, -12d), new Node2D(0.1, 0d)),
       //первый слой
       new(1, new Node2D(0.1, -3d), new Node2D(0.75, 0d)),
       new(2, new Node2D(0.75, -3d), new Node2D(1d, 0d)),
       //второй слой
       new(3, new Node2D(0.1, -6d), new Node2D(0.35, -3d)),
       new(4, new Node2D(0.35, -6d), new Node2D(0.65, -3d)),
       new(3, new Node2D(0.65, -6d), new Node2D(1d, -3d)),
       //третий слой
       new(3, new Node2D(0.1, -9d), new Node2D(1d, -6d)),
       //четвертый слой
       new(2, new Node2D(0.1, -12d), new Node2D(0.25, -9d)),
       new(1, new Node2D(0.25, -12d), new Node2D(1d, -9d)),
   })
   //.SetAreas(new Area[]
   //{
   //    //скважина
   //    new(0, new Node2D(1e-15, -12d), new Node2D(0.1, 0d)),
   //    //первый слой
   //    new(2, new Node2D(0.1, -3d), new Node2D(0.75, 0d)),
   //    new(2, new Node2D(0.75, -3d), new Node2D(1d, 0d)),
   //    //второй слой
   //    new(2, new Node2D(0.1, -6d), new Node2D(0.35, -3d)),
   //    new(4, new Node2D(0.35, -6d), new Node2D(0.65, -3d)),
   //    new(2, new Node2D(0.65, -6d), new Node2D(1d, -3d)),
   //    //третий слой
   //    new(2, new Node2D(0.1, -9d), new Node2D(1d, -6d)),
   //    //четвертый слой
   //    new(2, new Node2D(0.1, -12d), new Node2D(0.25, -9d)),
   //    new(2, new Node2D(0.25, -12d), new Node2D(1d, -9d)),
   //})
   //.SetAreas(new Area[]
   //{
   //    //скважина
   //    new(0, new Node2D(1e-15, -12d), new Node2D(0.1, 0d)),
   //    //первый слой
   //    new(2, new Node2D(0.1, -3d), new Node2D(0.75, 0d)),
   //    new(2, new Node2D(0.75, -3d), new Node2D(1d, 0d)),
   //    //второй слой
   //    new(2, new Node2D(0.1, -6d), new Node2D(0.35, -3d)),
   //    new(2, new Node2D(0.35, -6d), new Node2D(0.65, -3d)),
   //    new(2, new Node2D(0.65, -6d), new Node2D(1d, -3d)),
   //    //третий слой
   //    new(4, new Node2D(0.1, -9d), new Node2D(1d, -6d)),
   //    //четвертый слой
   //    new(2, new Node2D(0.1, -12d), new Node2D(0.25, -9d)),
   //    new(2, new Node2D(0.25, -12d), new Node2D(1d, -9d)),
   //})
   //.SetAreas(new Area[]
   //{
   //    //скважина
   //    new(6, new Node2D(1e-3, -12d), new Node2D(0.1, 0d)),
   //    //первый слой
   //    new(6, new Node2D(0.1, -3d), new Node2D(0.75, 0d)),
   //    new(6, new Node2D(0.75, -3d), new Node2D(1d, 0d)),
   //    //второй слой
   //    new(6, new Node2D(0.1, -6d), new Node2D(0.35, -3d)),
   //    new(6, new Node2D(0.35, -6d), new Node2D(0.65d, -3d)),
   //    new(6, new Node2D(0.65d, -6d), new Node2D(1d, -3d)),
   //    //третий слой
   //    new(6, new Node2D(0.1, -9d), new Node2D(1d, -6d)),
   //    //четвертый слой
   //    new(6, new Node2D(0.1, -12d), new Node2D(0.25, -9d)),
   //    new(6, new Node2D(0.25, -12d), new Node2D(1d, -9d)),
   //})
   .Build();

var materialFactory = new MaterialFactory
(
    new List<double> { 1d, 1d, 1d, 1d, 1d, 1d, 1d, 1d },
    new List<double> { 0.5, 0.1, 0.05, 0.2, 1d/3d, 0d, 1d }
);

var omegas = new[] { 4e4, 2e5, 1e6, 2e6 };
var current = 1d;

var sources = new FocusedSource[100];
var receiverLines = new ReceiverLine[100];
var emfs = new Complex[100, 4];
var phaseDifferences = new double[100, 4];
var centersZ = new double[100];

for (var i = 0; i < 100; i++)
{
    sources[i] = new FocusedSource(new Node2D(0.05, -1 - 0.1 * i), current);
    receiverLines[i] = new ReceiverLine(
        new Node2D(sources[i].Point.R, sources[i].Point.Z - 0.1), new Node2D(sources[i].Point.R, sources[i].Point.Z - 0.2)
    );
    centersZ[i] = (sources[i].Point.Z + receiverLines[i].PointN.Z) / 2;
}

var firstBoundaryProvider = new FirstBoundaryProvider(grid);
var conditions = firstBoundaryProvider.GetConditions(20, 40);

var directProblemSolver = new DirectProblemSolver(grid, materialFactory, conditions);

var resultO = new ResultIO("../DirectProblem/Results/");

var localBasisFunctionsProvider = new LocalBasisFunctionsProvider(grid, new LinearFunctionsProvider());

for (var i = 0; i < sources.Length; i++)
{
    for (var j = 0; j < omegas.Length; j++)
    {
        var solution = directProblemSolver
            .SetOmega(omegas[j])
            .SetSource(sources[i])
            .AssembleSLAE()
            .Solve();

        var femSolution = new FEMSolution(grid, solution, localBasisFunctionsProvider, omegas[j]);

        var potentialM = femSolution.Calculate(receiverLines[i].PointM);
        var potentialN = femSolution.Calculate(receiverLines[i].PointN);

        emfs[i, j] = 2 * Math.PI * receiverLines[i].PointM.R * potentialN;

        phaseDifferences[i, j] = potentialM.Phase - potentialN.Phase;

        //var magnitudeM = potentialM.Magnitude;
        //var magnitudeN = potentialN.Magnitude;

        //var potentialDifference = potentialM - potentialN;

        //resultO.Write($"omega{j} sin.txt", omegas[j], points, result);
        //resultO.Write($"omega{j} cos.txt", omegas[j], points, result);
    }
}

resultO.Write("emfs.txt", omegas, centersZ, emfs);
resultO.Write("phaseDifferences.txt", omegas, centersZ, phaseDifferences);