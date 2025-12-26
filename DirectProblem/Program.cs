using DirectProblem;
using DirectProblem.Core.GridComponents;
using DirectProblem.IO;
using DirectProblem.TwoDimensional;
using DirectProblem.TwoDimensional.Assembling.Local;
using System.Diagnostics;
using System.Globalization;
using System.Numerics;

Thread.CurrentThread.CurrentCulture = CultureInfo.InvariantCulture;

var grid = Grids.GetUniformGridWith0Dot003125StepWith2Materials();

var gridO = new GridIO("../DirectProblem/Results/");

//gridO.WriteMaterials(grid, "nvkat2d.dat");
//gridO.WriteElements(grid, "nvtr.dat");
//gridO.WriteNodes(grid, "rz.dat");

var mu = 4 * Math.PI * 1e-7;

var materials = new Material[]
{
    new(mu, 0.1),
    new(mu, 0.1),
    new(mu, 1d/30),
    new(mu, 0.01),
    new(mu, 1d / 3d),
    new(mu, 0.2),
    new(mu, 0.1),
    new(mu, 0.25),
    new(mu, 1),
};

//gridO.WriteAreas(grid, new Vector(materials.Select(m => m.Sigma).ToArray()), "true areas.txt");

var omegas = new[] { 4e4, 2e5, };
var current = 1;

var sources = new Source[2];
var receiverLines = new ReceiverLine[sources.Length];
var emfs = new Complex[omegas.Length, sources.Length];
var phaseDifferences = new double[omegas.Length, sources.Length];
var centersZ = new double[sources.Length];

for (var i = 0; i < sources.Length; i++)
{
    sources[i] = new Source(new Node2D(0.05, -2.5 - 0.5 * i), current);
    receiverLines[i] = new ReceiverLine(
        new Node2D(sources[i].Point.R, sources[i].Point.Z - 0.25),
        new Node2D(sources[i].Point.R, sources[i].Point.Z - 0.5)
    );
    centersZ[i] = (sources[i].Point.Z + receiverLines[i].PointN.Z) / 2;
}

var directProblemSolver = new DirectProblemSolver(grid, materials);

var resultO = new ResultIO("../DirectProblem/Results/");

var localBasisFunctionsProvider = new LocalBasisFunctionsProvider(grid);

var stopwatch = new Stopwatch();
stopwatch.Start();

for (var i = 0; i < omegas.Length; i++)
{
    for (var j = 0; j < sources.Length; j++)
    {
        var solution = directProblemSolver
            .SetDenseProvider(new DenseValuesProviderViaFunction(v => new Complex(0, 0), grid))
            .SetFrequency(omegas[i])
            .SetSource(sources[j])
            .AssembleSLAE()
            .Solve();

        var femSolution = new FEMSolution(grid, solution, localBasisFunctionsProvider);

        //if (i == 4 && j == 0)
        //{
        //    resultO.WriteSinuses(solution, "v2s.dat");
        //    resultO.WriteCosinuses(solution, "v2c.dat");
        //}

        var potentialM = femSolution.Calculate(receiverLines[j].PointM);
        var potentialN = femSolution.Calculate(receiverLines[j].PointN);

        emfs[i, j] = 2 * Math.PI * receiverLines[i].PointM.R * potentialM;

        phaseDifferences[i, j] = (potentialM.Phase - potentialN.Phase) * 180d / Math.PI;

        Console.Write($"source {i} frequency {j}                                   \r");
    }
}

stopwatch.Stop();

var time = (double)stopwatch.ElapsedMilliseconds / 1000;

Console.WriteLine();
Console.WriteLine($"Elapsed time {time}");

//resultO.Write("emfs.txt", omegas, centersZ, emfs);
//resultO.Write("phaseDifferences.txt", omegas, centersZ, phaseDifferences);

foreach (var phaseDifference in phaseDifferences)
{
    Console.WriteLine(phaseDifference);
}