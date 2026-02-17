using System.Diagnostics;
using System.Globalization;
using System.Numerics;
using Application.DirectProblem._2D.Cylindrical.Harmonic;
using Application.EquationSystems;
using Application.EquationSystems.MatrixDecompositions.LU;
using Application.EquationSystems.Preconditions.Separate;
using Application.EquationSystems.Solvers.Sparse;
using Application.FEM._2D.Assembling.Boundaries.First;
using Application.MathObjects.Equation;
using Application.MathObjects.Matrices;
using Domain.Nodes;
using Tests;

Thread.CurrentThread.CurrentCulture = CultureInfo.InvariantCulture;

//var globalMatrix = new SparseMatrix<double>
//(
//    [0, 0, 1, 2, 4],
//    [0, 0, 0, 1],
//    [10d, 10d, 10d, 10d],
//    [1d, 1d, 1d, 1d],
//    [2d, 2d, 2d, 2d]
//);

//var lu = new LU(new LUDecomposition());
//var rightPart = new Application.MathObjects.Vectors.Vector<double>([16d, 13d, 11d, 12d]);
//var sol = new Application.MathObjects.Vectors.Vector<double>(rightPart.Count);

//var profequation = new Equation<ISparseMatrix<double>, double>(globalMatrix, sol, rightPart);

//var soluti = lu.Solve(profequation);

var gridParameters = TestGridParameters.GetGridWith0Dot003125StepWithElementCloseToWellWith8Materials();
var materials = TestMaterials.GetMaterialsForGridWith0Dot003125StepWith8Materials();
var frequencies = TestFrequencies.GetTwoFrequencies();

const int sourcePower = 1;

var (sources, receiverLines) = TestReceiverAndSourceConstructions.GetTwoConstructions(sourcePower);

var emfs = new Complex[frequencies.Length, receiverLines.Length];
var phaseDifferences = new double[frequencies.Length, receiverLines.Length];
var centersZ = new double[receiverLines.Length];

for (var i = 0; i < sources.Length; i++)
{
    centersZ[i] = (sources[i].Location.Z() + receiverLines[i].ReceiverN.Z()) / 2;
}

//var directProblem = new HarmonicRotorDirectProblem2DFactory().Create(new LocalOptimalScheme(new LUPrecondition(new LUIncompleteDecomposition()), new IterativeMethodConfig()));
var directProblem = new HarmonicRotorDirectProblem2DFactory().Create(new LU(new LUDecomposition()));

var firstBoundaries =
    new ComplexDefinedValueFirstBoundaryProvider2D().GetOnAllBounds(gridParameters, new Complex(0, 0));

directProblem.GenerateGrid(gridParameters);
directProblem.SetMaterials(materials);
directProblem.SetDefinedValueBoundaryCondition(firstBoundaries);

var stopwatch = new Stopwatch();
stopwatch.Start();

for (var i = 0; i < frequencies.Length; i++)
{
    for (var j = 0; j < receiverLines.Length; j++)
    {
        directProblem.SetFrequency(frequencies[i]);
        directProblem.SetSources([sources[j]]);

        var solution = directProblem.Solve();

        var potentialM = solution.Get(receiverLines[j].ReceiverM);
        var potentialN = solution.Get(receiverLines[j].ReceiverN);

        emfs[i, j] = 2 * Math.PI * receiverLines[j].ReceiverM.R() * potentialM;

        phaseDifferences[i, j] = (potentialM.Phase - potentialN.Phase) * 180d / Math.PI;

        Console.Write($"source {i} frequency {j}                                   \r");
    }
}

stopwatch.Stop();

var time = (double)stopwatch.ElapsedMilliseconds / 1000;

Console.WriteLine();
Console.WriteLine($"Elapsed time {time}");

foreach (var phaseDifference in phaseDifferences)
{
    Console.WriteLine(phaseDifference);
}