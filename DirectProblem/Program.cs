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

var grid = gridBuilder2D
    .SetRAxis(new AxisSplitParameter(
            new[] { 1e-16, 0.1, 10 },
            new UniformSplitter(40),
            new ProportionalSplitter(63, 1.1)
        )
    )
    .SetZAxis(new AxisSplitParameter(
            new[] { -10d, -6d, -5d, -4d, 0d },
            new ProportionalSplitter(30, Math.Pow(0.95, 1)),
            new ProportionalSplitter(60, Math.Pow(0.95, 1)),
            new ProportionalSplitter(60, Math.Pow(1.05, 1)),
            new ProportionalSplitter(30, Math.Pow(1.05, 1))
        )
    )
   //искомый объект вплотную к скважине
   //.SetAreas(new Area[]
   //{
   //    //скважина
   //    new(0, new Node2D(1e-16, -10d), new Node2D(0.1, 0d)),
   //    //первый слой
   //    new(2, new Node2D(0.1, -4d), new Node2D(10d, 0d)),
   //    //второй слой
   //    new(1, new Node2D(0.1, -4.744557504251738), new Node2D(2.0390183336830607, -4d)),
   //    new(1, new Node2D(2.0390183336830607, -5d), new Node2D(10d, -4d)),
   //    //искомый элемент
   //    new(4, new Node2D(0.1, -5.2577958272949745), new Node2D(2.0390183336830607, -4.744557504251738)),
   //    //третий слой
   //    new(3, new Node2D(0.1, -6d), new Node2D(2.0390183336830607, -5.2577958272949745)),
   //    new(3, new Node2D(2.0390183336830607, -6d), new Node2D(10d, -5d)),
   //    //четвертый слой
   //    new(2, new Node2D(0.1, -10d), new Node2D(10d, -6d))
   //})
   //искомый объект близко к скважине (R = 10.1)
   //.SetAreas(new Area[]
   //{
   //    //скважина
   //    new(0, new Node2D(1e-16, -10d), new Node2D(0.1, 0d)),
   //    //первый слой
   //    new(2, new Node2D(0.1, -4d), new Node2D(10, 0d)),
   //    //второй слой
   //    new(1, new Node2D(0.1, -5d), new Node2D(0.9915017244268906, -4d)),
   //    new(1, new Node2D(0.9915017244268906, -4.744557504251738), new Node2D(2.950282033163873, -4d)),
   //    new(1, new Node2D(2.950282033163873, -5), new Node2D(10, -4d)),
   //    //искомый элемент
   //    new(4, new Node2D(0.9915017244268906, -5.2577958272949745), new Node2D(2.950282033163873, -4.744557504251738)),
   //    //третий слой
   //    new(3, new Node2D(0.1, -6d), new Node2D(0.9915017244268906, -5d)),
   //    new(3, new Node2D(0.9915017244268906, -6d), new Node2D(2.950282033163873, -5.2577958272949745)),
   //    new(3, new Node2D(2.950282033163873, -6d), new Node2D(10, -5d)),
   //    //четвертый слой
   //    new(2, new Node2D(0.1, -10d), new Node2D(10d, -6d))
   //})
   //искомый объект далеко от скважины (R = 40.1)
   //.SetAreas(new Area[]
   //{
   //    //скважина
   //    new(0, new Node2D(1e-16, -10d), new Node2D(0.1, 0d)),
   //    //первый слой
   //    new(2, new Node2D(0.1, -4d), new Node2D(10, 0d)),
   //    //второй слой
   //    new(1, new Node2D(0.1, -5d), new Node2D(3.901831206569741, -4d)),
   //    new(1, new Node2D(3.901831206569741, -4.744557504251738), new Node2D(6.237837884740006, -4d)),
   //    new(1, new Node2D(6.237837884740006, -5d), new Node2D(10, -4d)),
   //    //искомый элемент
   //    new(4, new Node2D(3.901831206569741, -5.2577958272949745), new Node2D(6.237837884740006, -4.744557504251738)),
   //    //третий слой
   //    new(3, new Node2D(0.1, -6d), new Node2D(3.901831206569741, -5d)),
   //    new(3, new Node2D(3.901831206569741, -6d), new Node2D(6.237837884740006, -5.2577958272949745)),
   //    new(3, new Node2D(6.237837884740006, -6d), new Node2D(10, -5d)),
   //    //четвертый слой
   //    new(2, new Node2D(0.1, -10d), new Node2D(10d, -6d))
   //})
   //искомые объекты вплотную к скважине
   //.SetAreas(new Area[]
   //{
   //    //скважина
   //    new(0, new Node2D(1e-16, -10d), new Node2D(0.1, 0d)),
   //    //первый слой
   //    new(2, new Node2D(0.1, -4d), new Node2D(10d, 0d)),
   //    //второй слой
   //    new(1, new Node2D(0.1, -4.268139597133065), new Node2D(2.0390183336830607, -4d)),
   //    new(1, new Node2D(0.1, -5d), new Node2D(2.0390183336830607, -4.744557504251738)),
   //    new(1, new Node2D(2.0390183336830607, -5d), new Node2D(10d, -4d)),
   //    //искомый элемент
   //    new(4, new Node2D(0.1, -4.744557504251738), new Node2D(2.0390183336830607, -4.268139597133065)),
   //    //искомый элемент
   //    new(4, new Node2D(0.1, -5.762855749550898), new Node2D(2.0390183336830607, -5.2577958272949745)),
   //    //третий слой
   //    new(3, new Node2D(0.1, -6d), new Node2D(2.0390183336830607, -5.762855749550898)),
   //    new(3, new Node2D(0.1, -5.2577958272949745), new Node2D(2.0390183336830607, -5d)),
   //    new(3, new Node2D(2.0390183336830607, -6d), new Node2D(10d, -5d)),
   //    //четвертый слой
   //    new(2, new Node2D(0.1, -10d), new Node2D(10d, -6d))
   //})
   //искомые объекы близко к скважине (R = 10.1)
   //.SetAreas(new Area[]
   //{
   //    //скважина
   //    new(0, new Node2D(1e-16, -10d), new Node2D(0.1, 0d)),
   //    //первый слой
   //    new(2, new Node2D(0.1, -4d), new Node2D(10d, 0d)),
   //    //второй слой
   //    new(1, new Node2D(0.1, -5d), new Node2D(0.9915017244268906, -4d)),
   //    new(1, new Node2D(0.9915017244268906, -5d), new Node2D(2.950282033163873, -4.744557504251738)),
   //    new(1, new Node2D(0.9915017244268906, -4.268139597133065), new Node2D(2.950282033163873, -4d)),
   //    new(1, new Node2D(2.950282033163873, -5d), new Node2D(10d, -4d)),
   //    //искомый элемент
   //    new(4, new Node2D(0.9915017244268906, -4.744557504251738), new Node2D(2.950282033163873, -4.268139597133065)),
   //    //искомый элемент
   //    new(4, new Node2D(0.9915017244268906, -5.762855749550898), new Node2D(2.950282033163873, -5.2577958272949745)),
   //    //третий слой
   //    new(3, new Node2D(0.1, -6d), new Node2D(0.9915017244268906, -5d)),
   //    new(3, new Node2D(0.9915017244268906, -6d), new Node2D(2.950282033163873, -5.762855749550898)),
   //    new(3, new Node2D(0.9915017244268906, -5.2577958272949745), new Node2D(2.950282033163873, -5d)),
   //    new(3, new Node2D(2.950282033163873, -6d), new Node2D(10d, -5d)),
   //    //четвертый слой
   //    new(2, new Node2D(0.1, -10d), new Node2D(10d, -6d))
   //})
   //искомые объекты далеко от скважины (R = 40.1)
   .SetAreas(new Area[]
   {
       //скважина
       new(0, new Node2D(1e-16, -10d), new Node2D(0.1, 0d)),
       //первый слой
       new(2, new Node2D(0.1, -4d), new Node2D(10d, 0d)),
       //второй слой
       new(1, new Node2D(0.1, -5d), new Node2D(3.901831206569741, -4d)),
       new(1, new Node2D(3.901831206569741, -5d), new Node2D(6.237837884740006, -4.744557504251738)),
       new(1, new Node2D(3.901831206569741, -4.268139597133065), new Node2D(6.237837884740006, -4d)),
       new(1, new Node2D(6.237837884740006, -5d), new Node2D(10d , -4d)),
       //искомый элемент
       new(4, new Node2D(3.901831206569741, -4.744557504251738), new Node2D(6.237837884740006, -4.268139597133065)),
       //искомый элемент
       new(4, new Node2D(3.901831206569741, -5.762855749550898), new Node2D(6.237837884740006, -5.2577958272949745)),
       //третий слой
       new(3, new Node2D(0.1, -6d), new Node2D(3.901831206569741, -5d)),
       new(3, new Node2D(3.901831206569741, -6d), new Node2D(6.237837884740006, -5.762855749550898)),
       new(3, new Node2D(3.901831206569741, -5.2577958272949745), new Node2D(6.237837884740006, -5d)),
       new(3, new Node2D(6.237837884740006, -6d), new Node2D(10d, -5d)),
       //четвертый слой
       new(2, new Node2D(0.1, -10d), new Node2D(10d, -6d))
   })
   //.SetAreas(new Area[]
   //{
   //    new(6, new Node2D(1e-16, -10d), new Node2D(10d, 0d)),
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
var current = 1;

var sources = new FocusedSource[40];
var receiverLines = new ReceiverLine[sources.Length];
var emfs = new Complex[sources.Length, omegas.Length];
var phaseDifferences = new double[sources.Length, omegas.Length];
var centersZ = new double[sources.Length];

for (var i = 0; i < sources.Length; i++)
{
    sources[i] = new FocusedSource(new Node2D(0.05, -4 - 0.05 * i), current);
    receiverLines[i] = new ReceiverLine(
        new Node2D(sources[i].Point.R, sources[i].Point.Z - 0.05), new Node2D(sources[i].Point.R, sources[i].Point.Z - 0.1)
    );
    centersZ[i] = (sources[i].Point.Z + receiverLines[i].PointN.Z) / 2;
}

var firstBoundaryProvider = new FirstBoundaryProvider(grid);
var conditions = firstBoundaryProvider.GetConditions(103, 180);

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

        if (i == 20 && j == 0)
        {
            resultO.WriteSinuses(solution, "v2s.dat");
            resultO.WriteCosinuses(solution, "v2c.dat");
        }

        var potentialM = femSolution.Calculate(receiverLines[i].PointM);
        var potentialN = femSolution.Calculate(receiverLines[i].PointN);

        emfs[i, j] = 2 * Math.PI * receiverLines[i].PointM.R * potentialN;

        phaseDifferences[i, j] = (potentialM.Phase - potentialN.Phase) * 180d / Math.PI;

        CourseHolder.GetInfo(i, j);
    }
}

resultO.Write("emfs.txt", omegas, centersZ, emfs);
resultO.Write("phaseDifferences.txt", omegas, centersZ, phaseDifferences);