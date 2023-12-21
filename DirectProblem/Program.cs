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

//-110.24319388881395
//-120.03858175448441
//-140.11800433374964
//-149.95696963486563
//0.5, 0.1, 0.05, 0.2, 1d / 3, 0d, 1d

var grid = gridBuilder2D
   .SetRAxis(new AxisSplitParameter(
           new[] { 1e-3, 0.1, 100.1 },
           new UniformSplitter(2),
           new UniformSplitter(10)
       )
   )
   .SetZAxis(new AxisSplitParameter(
           new[] { -260d, -160d, -130d, -100d, 0d },
           new UniformSplitter(100),
           new ProportionalSplitter(120, Math.Pow(0.95, 0.5)),
           new ProportionalSplitter(120, Math.Pow(1.05, 0.5)),
           new UniformSplitter(100)
       )
   )
   //искомый объект вплотную к скважине
   //.SetAreas(new Area[]
   //{
   //    //скважина
   //    new(0, new Node2D(1e-16, -260d), new Node2D(0.1, 0d)),
   //    //первый слой
   //    new(1, new Node2D(0.1, -100d), new Node2D(70.1, 0d)),
   //    new(3, new Node2D(70.1, -100d), new Node2D(100.1, 0d)),
   //    //второй слой
   //    new(3, new Node2D(0.1, -120.03858175448441), new Node2D(20.1, -100d)),
   //    new(3, new Node2D(20.1, -130d), new Node2D(60.1, -100d)),
   //    new(1, new Node2D(60.1, -130d), new Node2D(100.1, -100d)),
   //    //искомый элемент
   //    new(4, new Node2D(0.1, -140.11800433374964), new Node2D(20.1, -120.03858175448441)),
   //    //третий слой
   //    new(2, new Node2D(0.1, -160d), new Node2D(20.1, -140.11800433374964)),
   //    new(2, new Node2D(20.1, -160d), new Node2D(100.1, -130d)),
   //    //четвертый слой
   //    new(3, new Node2D(0.1, -260d), new Node2D(30.1, -160d)),
   //    new(1, new Node2D(30.1, -260d), new Node2D(100.1, -160d))
   //})
   //искомый объект близко к скважине (R = 10.1)
   //.SetAreas(new Area[]
   //{
   //    //скважина
   //    new(0, new Node2D(1e-16, -260d), new Node2D(0.1, 0d)),
   //    //первый слой
   //    new(1, new Node2D(0.1, -100d), new Node2D(70.1, 0d)),
   //    new(3, new Node2D(70.1, -100d), new Node2D(100.1, 0d)),
   //    //второй слой
   //    new(3, new Node2D(0.1, -130d), new Node2D(10.1, -100d)),
   //    new(3, new Node2D(10.1, -120.03858175448441), new Node2D(30.1, -100d)),
   //    new(3, new Node2D(30.1, -130d), new Node2D(60.1, -100d)),
   //    new(1, new Node2D(60.1, -130d), new Node2D(100.1, -100d)),
   //    //искомый элемент
   //    new(4, new Node2D(10.1, -140.11800433374964), new Node2D(30.1, -120.03858175448441)),
   //    //третий слой
   //    new(2, new Node2D(0.1, -160d), new Node2D(10.1, -130d)),
   //    new(2, new Node2D(10.1, -160d), new Node2D(30.1, -140.11800433374964)),
   //    new(2, new Node2D(30.1, -160d), new Node2D(100.1, -130d)),
   //    //четвертый слой
   //    new(3, new Node2D(0.1, -260d), new Node2D(30.1, -160d)),
   //    new(1, new Node2D(30.1, -260d), new Node2D(100.1, -160d))
   //})
   //искомый объект далеко от скважины (R = 40.1)
   //.SetAreas(new Area[]
   //{
   //    //скважина
   //    new(0, new Node2D(1e-16, -260d), new Node2D(0.1, 0d)),
   //    //первый слой
   //    new(1, new Node2D(0.1, -100d), new Node2D(70.1, 0d)),
   //    new(3, new Node2D(70.1, -100d), new Node2D(100.1, 0d)),
   //    //второй слой
   //    new(3, new Node2D(0.1, -130d), new Node2D(40.1, -100d)),
   //    new(3, new Node2D(40.1, -120.03858175448441), new Node2D(60.1, -100d)),
   //    new(1, new Node2D(60.1, -130d), new Node2D(100.1, -100d)),
   //    //искомый элемент
   //    new(4, new Node2D(40.1, -140.11800433374964), new Node2D(60.1, -120.03858175448441)),
   //    //третий слой
   //    new(2, new Node2D(0.1, -160d), new Node2D(40.1, -130d)),
   //    new(2, new Node2D(40.1, -160d), new Node2D(60.1, -140.11800433374964)),
   //    new(2, new Node2D(60.1, -160d), new Node2D(100.1, -130d)),
   //    //четвертый слой
   //    new(3, new Node2D(0.1, -260d), new Node2D(30.1, -160d)),
   //    new(1, new Node2D(30.1, -260d), new Node2D(100.1, -160d))
   //})
   //искомые объекты вплотную к скважине
   .SetAreas(new Area[]
   {
       //скважина
       new(0, new Node2D(1e-3, -260d), new Node2D(0.1, 0d)),
       //первый слой
       new(1, new Node2D(0.1, -100d), new Node2D(70.1, 0d)),
       new(3, new Node2D(70.1, -100d), new Node2D(100.1, 0d)),
       //второй слой
       new(3, new Node2D(0.1, -120.03858175448441), new Node2D(20.1, -100d)),
       new(3, new Node2D(20.1, -130d), new Node2D(60.1, -100d)),
       new(1, new Node2D(60.1, -130d), new Node2D(100.1, -100d)),
       //искомый элемент
       new(4, new Node2D(0.1, -130d), new Node2D(20.1, -120.03858175448441)),
       //искомый элемент
       new(4, new Node2D(0.1, -149.95696963486563), new Node2D(20.1, -140.11800433374964)),
       //третий слой
       new(2, new Node2D(0.1, -140.11800433374964), new Node2D(20.1, -130d)),
       new(2, new Node2D(0.1, -160d), new Node2D(20.1, -149.95696963486563)),
       new(2, new Node2D(0.1, -160d), new Node2D(20.1, -130d)),
       new(2, new Node2D(20.1, -160d), new Node2D(100.1, -130d)),
       //четвертый слой
       new(3, new Node2D(0.1, -260d), new Node2D(30.1, -160d)),
       new(1, new Node2D(30.1, -260d), new Node2D(100.1, -160d))
   })
   //искомые объекы близко к скважине (R = 10.1)
   //.SetAreas(new Area[]
   //{
   //    //скважина
   //    new(0, new Node2D(1e-16, -260d), new Node2D(0.1, 0d)),
   //    //первый слой
   //    new(1, new Node2D(0.1, -100d), new Node2D(70.1, 0d)),
   //    new(3, new Node2D(70.1, -100d), new Node2D(100.1, 0d)),
   //    //второй слой
   //    new(3, new Node2D(0.1, -130d), new Node2D(10.1, -100d)),
   //    new(3, new Node2D(10.1, -120.03858175448441), new Node2D(30.1, -100d)),
   //    new(3, new Node2D(30.1, -130d), new Node2D(60.1, -100d)),
   //    new(2, new Node2D(60.1, -130d), new Node2D(100.1, -100d)),
   //    //искомый элемент
   //    new(4, new Node2D(10.1, -130d), new Node2D(30.1, -120.03858175448441)),
   //    //искомый элемент
   //    new(4, new Node2D(10.1, -149.95696963486563), new Node2D(30.1, -140.11800433374964)),
   //    //третий слой
   //    new(2, new Node2D(0.1, -160d), new Node2D(10.1, -130d)),
   //    new(2, new Node2D(10.1, -140.11800433374964), new Node2D(30.1, -130d)),
   //    new(2, new Node2D(10.1, -160d), new Node2D(30.1, -149.95696963486563)),
   //    new(2, new Node2D(30.1, -160d), new Node2D(100.1, -130d)),
   //    //четвертый слой
   //    new(3, new Node2D(0.1, -260d), new Node2D(30.1, -160d)),
   //    new(1, new Node2D(30.1, -260d), new Node2D(100.1, -160d))
   //})
   //искомые объекты далеко от скважины (R = 40.1)
   //.SetAreas(new Area[]
   //{
   //    //скважина
   //    new(0, new Node2D(1e-16, -260d), new Node2D(0.1, 0d)),
   //    //первый слой
   //    new(1, new Node2D(0.1, -100d), new Node2D(70.1, 0d)),
   //    new(3, new Node2D(70.1, -100d), new Node2D(100.1, 0d)),
   //    //второй слой
   //    new(3, new Node2D(0.1, -130d), new Node2D(40.1, -100d)),
   //    new(3, new Node2D(40.1, -120.03858175448441), new Node2D(60.1, -100d)),
   //    new(1, new Node2D(60.1, -130d), new Node2D(100.1, -100d)),
   //    //искомый элемент
   //    new(4, new Node2D(40.1, -130d), new Node2D(60.1, -120.03858175448441)),
   //    //искомый элемент
   //    new(4, new Node2D(40.1, -149.95696963486563), new Node2D(60.1, -140.11800433374964)),
   //    //третий слой
   //    new(2, new Node2D(0.1, -160d), new Node2D(40.1, -130d)),
   //    new(2, new Node2D(40.1, -140.11800433374964), new Node2D(60.1, -130d)),
   //    new(2, new Node2D(40.1, -160d), new Node2D(60.1, -149.95696963486563)),
   //    new(2, new Node2D(60.1, -160d), new Node2D(100.1, -130d)),
   //    //четвертый слой
   //    new(3, new Node2D(0.1, -260d), new Node2D(30.1, -160d)),
   //    new(1, new Node2D(30.1, -260d), new Node2D(100.1, -160d))
   //})
   //.SetAreas(new Area[]
   //{
   //    new(6, new Node2D(1e-16, -260d), new Node2D(100.1, 0d)),
   //})
   .Build();

var materialFactory = new MaterialFactory
(
    new List<double> { 1d, 1d, 1d, 1d, 1d, 1d, 1d, 1d },
    new List<double> { 0.5, 0.1, 0.05, 0.2, 1d/3d, 0d, 1d }
);

var omegas = new[] { 4e4, 2e5, 1e6, 2e6 };
var current = 1d;

var sources = new FocusedSource[59];
var receiverLines = new ReceiverLine[59];
var emfs = new Complex[59, 4];
var phaseDifferences = new double[59, 4];
var centersZ = new double[59];

for (var i = 0; i < 59; i++)
{
    sources[i] = new FocusedSource(new Node2D(0.05, -100 - 1 * i), current);
    receiverLines[i] = new ReceiverLine(
        new Node2D(sources[i].Point.R, sources[i].Point.Z - 1), new Node2D(sources[i].Point.R, sources[i].Point.Z - 2)
    );
    centersZ[i] = (sources[i].Point.Z + receiverLines[i].PointN.Z) / 2;
}

var firstBoundaryProvider = new FirstBoundaryProvider(grid);
var conditions = firstBoundaryProvider.GetConditions(12, 440);

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

        phaseDifferences[i, j] = (potentialM.Phase - potentialN.Phase) * 180d / Math.PI;

        //var magnitudeM = potentialM.Magnitude;
        //var magnitudeN = potentialN.Magnitude;

        //var potentialDifference = potentialM - potentialN;

        //resultO.Write($"omega{j} sin.txt", omegas[j], points, result);
        //resultO.Write($"omega{j} cos.txt", omegas[j], points, result);
    }
}

resultO.Write("emfs.txt", omegas, centersZ, emfs);
resultO.Write("phaseDifferences.txt", omegas, centersZ, phaseDifferences);