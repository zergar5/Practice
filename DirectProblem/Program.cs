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
using System.Diagnostics;
using System.Runtime.CompilerServices;

Thread.CurrentThread.CurrentCulture = CultureInfo.InvariantCulture;

var grid = Grids.GetSmallGridWith0Dot003125StepWithElementsCloseAndNearToWell();

var gridO = new GridIO("../DirectProblem/Results/");

gridO.WriteMaterials(grid, "nvkat2d.dat");
gridO.WriteElements(grid, "nvtr.dat");
gridO.WriteNodes(grid, "rz.dat");

var mu = 4 * Math.PI * 10e-7;

var materialFactory = new MaterialFactory
(
    new List<double> { mu, mu, mu, mu, mu, mu, mu },
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
    sources[i] = new FocusedSource(new Node2D(0.05, -2 - 0.05 * i), current);
    receiverLines[i] = new ReceiverLine(
        new Node2D(sources[i].Point.R, sources[i].Point.Z - 0.05), new Node2D(sources[i].Point.R, sources[i].Point.Z - 0.1)
    );
    centersZ[i] = (sources[i].Point.Z + receiverLines[i].PointN.Z) / 2;
}

var firstBoundaryProvider = new FirstBoundaryProvider(grid);
var conditions = firstBoundaryProvider.GetConditions(grid.Nodes.RLength - 1, grid.Nodes.ZLength - 1);

var directProblemSolver = new DirectProblemSolver(grid, materialFactory, conditions);

var resultO = new ResultIO("../DirectProblem/Results/");

var localBasisFunctionsProvider = new LocalBasisFunctionsProvider(grid, new LinearFunctionsProvider());

var stopwatch = new Stopwatch();
stopwatch.Start();

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

        emfs[i, j] = 2 * Math.PI * receiverLines[i].PointM.R * potentialM;

        phaseDifferences[i, j] = (potentialM.Phase - potentialN.Phase) * 180d / Math.PI;

        //if (i == 20 && j == 0)
        //{
        //    Console.WriteLine(femSolution.Calculate(receiverLines[i].PointM));
        //    Console.WriteLine((potentialM.Phase - potentialN.Phase) * 180d / Math.PI);
        //    break;
        //}

        Console.Write($"source {i} frequency {j}                                   \r");
    }
}

stopwatch.Stop();

var time = (double)stopwatch.ElapsedMilliseconds / 1000;

Console.WriteLine();
Console.WriteLine($"Elapsed time {time}");

resultO.Write("emfs.txt", omegas, centersZ, emfs);
resultO.Write("phaseDifferences.txt", omegas, centersZ, phaseDifferences);