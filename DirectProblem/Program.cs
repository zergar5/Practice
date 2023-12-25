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
using System.Xml.XPath;

Thread.CurrentThread.CurrentCulture = CultureInfo.InvariantCulture;

var gridBuilder2D = new GridBuilder2D();

//-110.24319388881395
//-120.03858175448441
//-124.9308507937855
//-135.1008421814358
//-140.11800433374964
//-149.95696963486563
//0.5, 0.1, 0.05, 0.2, 1d / 3, 0d, 1d

var grid = gridBuilder2D
   .SetRAxis(new AxisSplitParameter(
           new[] { 1e-16, 0.1, 100.1 },
           new UniformSplitter(10),
           //new ProportionalSplitter(41, 1.2)
           new ProportionalSplitter(72, 1.1)
       )
   )
   .SetZAxis(new AxisSplitParameter(
           new[] { -260d, -160d, -130d, -100d, 0d },
           new ProportionalSplitter(95, Math.Pow(0.95, 0.25)),
           new ProportionalSplitter(110, Math.Pow(0.95, 0.25)),
           new ProportionalSplitter(110, Math.Pow(1.05, 0.25)),
           new ProportionalSplitter(95, Math.Pow(1.05, 0.25))
       )
   )
   //искомый объект вплотную к скважине
   //.SetAreas(new Area[]
   //{
   //    //скважина
   //    new(0, new Node2D(1e-16, -260d), new Node2D(0.1, 0d)),
   //    //первый слой
   //    new(2, new Node2D(0.1, -100d), new Node2D(100.1, 0d)),
   //    //второй слой
   //    new(1, new Node2D(0.1, -124.9308507937855), new Node2D(19.8004358271464, -100d)),
   //    new(1, new Node2D(19.8004358271464, -130d), new Node2D(100.1, -100d)),
   //    //искомый элемент
   //    new(4, new Node2D(0.1, -135.1008421814358), new Node2D(19.8004358271464, -124.9308507937855)),
   //    //третий слой
   //    new(3, new Node2D(0.1, -160d), new Node2D(19.8004358271464, -135.1008421814358)),
   //    new(3, new Node2D(19.8004358271464, -160d), new Node2D(100.1, -130d)),
   //    //четвертый слой
   //    new(2, new Node2D(0.1, -260d), new Node2D(100.1, -160d))
   //})
   //искомый объект близко к скважине (R = 10.1)
   //.SetAreas(new Area[]
   //{
   //    //скважина
   //    new(0, new Node2D(1e-16, -260d), new Node2D(0.1, 0d)),
   //    //первый слой
   //    new(2, new Node2D(0.1, -100d), new Node2D(100.1, 0d)),
   //    //второй слой
   //    new(1, new Node2D(0.1, -130d), new Node2D(10.15843867850181, -100d)),
   //    new(1, new Node2D(10.15843867850181, -124.9308507937855), new Node2D(28.99202563125968, -100d)),
   //    new(1, new Node2D(28.99202563125968, -130d), new Node2D(100.1, -100d)),
   //    //искомый элемент
   //    new(4, new Node2D(10.15843867850181, -135.1008421814358), new Node2D(28.99202563125968, -124.9308507937855)),
   //    //третий слой
   //    new(3, new Node2D(0.1, -160d), new Node2D(10.15843867850181, -130d)),
   //    new(3, new Node2D(10.15843867850181, -160d), new Node2D(28.99202563125968, -135.1008421814358)),
   //    new(3, new Node2D(28.99202563125968, -160d), new Node2D(100.1, -130d)),
   //    //четвертый слой
   //    new(2, new Node2D(0.1, -260d), new Node2D(100.1, -160d))
   //})
   //искомый объект далеко от скважины (R = 40.1)
   //.SetAreas(new Area[]
   //{
   //    //скважина
   //    new(0, new Node2D(1e-16, -260d), new Node2D(0.1, 0d)),
   //    //первый слой
   //    new(2, new Node2D(0.1, -100d), new Node2D(100.1, 0d)),
   //    //второй слой
   //    new(1, new Node2D(0.1, -130d), new Node2D(38.58996054886137, -100d)),
   //    new(1, new Node2D(38.58996054886137, -124.9308507937855), new Node2D(62.15242131366925, -100d)),
   //    new(1, new Node2D(62.15242131366925, -130d), new Node2D(100.1, -100d)),
   //    //искомый элемент
   //    new(4, new Node2D(38.58996054886137, -135.1008421814358), new Node2D(62.15242131366925, -124.9308507937855)),
   //    //третий слой
   //    new(3, new Node2D(0.1, -160d), new Node2D(38.58996054886137, -130d)),
   //    new(3, new Node2D(38.58996054886137, -160d), new Node2D(62.15242131366925, -135.1008421814358)),
   //    new(3, new Node2D(62.15242131366925, -160d), new Node2D(100.1, -130d)),
   //    //четвертый слой
   //    new(2, new Node2D(0.1, -260d), new Node2D(100.1, -160d))
   //})
   //искомые объекты вплотную к скважине
   //.SetAreas(new Area[]
   //{
   //    //скважина
   //    new(0, new Node2D(1e-16, -260d), new Node2D(0.1, 0d)),
   //    //первый слой
   //    new(2, new Node2D(0.1, -100d), new Node2D(100.1, 0d)),
   //    //второй слой
   //    new(1, new Node2D(0.1, -122.45831995387395), new Node2D(19.8004358271464, -100d)),
   //    new(1, new Node2D(0.1, -130d), new Node2D(19.8004358271464, -127.55359429507054)),
   //    new(1, new Node2D(19.8004358271464, -130d), new Node2D(100.1, -100d)),
   //    //искомый элемент
   //    new(4, new Node2D(0.1, -127.55359429507054), new Node2D(19.8004358271464, -122.45831995387395)),
   //    //искомый элемент
   //    new(4, new Node2D(0.1, -137.56011500378185), new Node2D(19.8004358271464, -132.51397105815582)),
   //    //третий слой
   //    new(3, new Node2D(0.1, -132.51397105815582), new Node2D(19.8004358271464, -130d)),
   //    new(3, new Node2D(0.1, -160d), new Node2D(19.8004358271464, -137.56011500378185)),
   //    new(3, new Node2D(19.8004358271464, -160d), new Node2D(100.1, -130d)),
   //    //четвертый слой
   //    new(2, new Node2D(0.1, -260d), new Node2D(100.1, -160d))
   //})
   //искомые объекы близко к скважине (R = 10.1)
   //.SetAreas(new Area[]
   //{
   //    //скважина
   //    new(0, new Node2D(1e-16, -260d), new Node2D(0.1, 0d)),
   //    //первый слой
   //    new(2, new Node2D(0.1, -100d), new Node2D(100.1, 0d)),
   //    //второй слой
   //    new(1, new Node2D(0.1, -130d), new Node2D(10.15843867850181, -100d)),
   //    new(1, new Node2D(10.15843867850181, -122.45831995387395), new Node2D(28.99202563125968, -100d)),
   //    new(1, new Node2D(10.15843867850181, -130d), new Node2D(28.99202563125968, -127.55359429507054)),
   //    new(1, new Node2D(28.99202563125968, -130d), new Node2D(100.1, -100d)),
   //    //искомый элемент
   //    new(4, new Node2D(10.15843867850181, -127.55359429507054), new Node2D(28.99202563125968, -122.45831995387395)),
   //    //искомый элемент
   //    new(4, new Node2D(10.15843867850181, -137.56011500378185), new Node2D(28.99202563125968, -132.51397105815582)),
   //    //третий слой
   //    new(3, new Node2D(0.1, -160d), new Node2D(10.15843867850181, -130d)),
   //    new(3, new Node2D(10.15843867850181, -132.51397105815582), new Node2D(28.99202563125968, -130d)),
   //    new(3, new Node2D(10.15843867850181, -160d), new Node2D(28.99202563125968, -137.56011500378185)),
   //    new(3, new Node2D(28.99202563125968, -160d), new Node2D(100.1, -130d)),
   //    //четвертый слой
   //    new(2, new Node2D(0.1, -260d), new Node2D(100.1, -160d))
   //})
   //искомые объекты далеко от скважины (R = 40.1)
   .SetAreas(new Area[]
   {
       //скважина
       new(0, new Node2D(1e-16, -260d), new Node2D(0.1, 0d)),
       //первый слой
       new(2, new Node2D(0.1, -100d), new Node2D(100.1, 0d)),
       //второй слой
       new(1, new Node2D(0.1, -130d), new Node2D(38.58996054886137, -100d)),
       new(1, new Node2D(38.58996054886137, -122.45831995387395), new Node2D(62.15242131366925, -100d)),
       new(1, new Node2D(38.58996054886137, -130d), new Node2D(62.15242131366925, -127.55359429507054)),
       new(1, new Node2D(62.15242131366925, -130d), new Node2D(100.1, -100d)),
       //искомый элемент
       new(4, new Node2D(38.58996054886137, -127.55359429507054), new Node2D(62.15242131366925, -122.45831995387395)),
       //искомый элемент
       new(4, new Node2D(38.58996054886137, -137.56011500378185), new Node2D(62.15242131366925, -132.51397105815582)),
       //третий слой
       new(3, new Node2D(0.1, -160d), new Node2D(38.58996054886137, -130d)),
       new(3, new Node2D(38.58996054886137, -132.51397105815582), new Node2D(62.15242131366925, -130d)),
       new(3, new Node2D(38.58996054886137, -160d), new Node2D(62.15242131366925, -137.56011500378185)),
       new(3, new Node2D(62.15242131366925, -160d), new Node2D(100.1, -130d)),
       //четвертый слой
       new(2, new Node2D(0.1, -260d), new Node2D(100.1, -160d))
   })
   //.SetAreas(new Area[]
   //{
   //    new(6, new Node2D(1e-16, -260d), new Node2D(100.1, 0d)),
   //})
   .Build();

var gridO = new GridIO("../DirectProblem/Results/");

gridO.WriteMaterials(grid, "nvkat2d.dat");
gridO.WriteElements(grid, "nvtr.dat");
gridO.WriteNodes(grid, "rz.dat");

var materialFactory = new MaterialFactory
(
    new List<double> { 1d, 1d, 1d, 1d, 1d, 1d, 1d, 1d },
    new List<double> { 0.5, 0.1, 0.05, 0.2, 1d/3d, 0d, 1d }
);

var omegas = new[] { 4e4, 2e5, 1e6, 2e6 };
var current = 1e13;

var sources = new FocusedSource[21];
var receiverLines = new ReceiverLine[21];
var emfs = new Complex[21, 4];
var phaseDifferences = new double[21, 4];
var centersZ = new double[21];

for (var i = 0; i < 21; i++)
{
    sources[i] = new FocusedSource(new Node2D(0.05, -119 - 1 * i), current);
    receiverLines[i] = new ReceiverLine(
        new Node2D(sources[i].Point.R, sources[i].Point.Z - 1), new Node2D(sources[i].Point.R, sources[i].Point.Z - 2)
    );
    centersZ[i] = (sources[i].Point.Z + receiverLines[i].PointN.Z) / 2;
}

var firstBoundaryProvider = new FirstBoundaryProvider(grid);
var conditions = firstBoundaryProvider.GetConditions(82, 410);

var directProblemSolver = new DirectProblemSolver(grid, materialFactory, conditions);

var resultO = new ResultIO("../DirectProblem/Results/");

var localBasisFunctionsProvider = new LocalBasisFunctionsProvider(grid, new LinearFunctionsProvider());

//for (var i = 0; i < sources.Length; i++)
//{
//    for (var j = 0; j < omegas.Length; j++)
//    {
        var solution = directProblemSolver
            .SetOmega(omegas[0])
            .SetSource(sources[11])
            .AssembleSLAE()
            .Solve();

        //var femSolution = new FEMSolution(grid, solution, localBasisFunctionsProvider, omegas[j]);

        resultO.WriteSinuses(solution, "v2s.dat");
        resultO.WriteCosinuses(solution, "v2c.dat");

        //var potentialM = femSolution.Calculate(receiverLines[i].PointM);
        //var potentialN = femSolution.Calculate(receiverLines[i].PointN);

        //emfs[i, j] = 2 * Math.PI * receiverLines[i].PointM.R * potentialN;

        //phaseDifferences[i, j] = (potentialM.Phase - potentialN.Phase) * 180d / Math.PI;

        //var magnitudeM = potentialM.Magnitude;
        //var magnitudeN = potentialN.Magnitude;

        //var potentialDifference = potentialM - potentialN;

        //resultO.Write($"omega{j} sin.txt", omegas[j], points, result);
        //resultO.Write($"omega{j} cos.txt", omegas[j], points, result);
//    }
//}

//resultO.Write("emfs.txt", omegas, centersZ, emfs);
//resultO.Write("phaseDifferences.txt", omegas, centersZ, phaseDifferences);