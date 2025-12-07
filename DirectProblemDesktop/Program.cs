using Application.DirectProblem._2D.Cylindrical.Harmonic;
using Application.EquationSystems;
using Application.FEM._1D.Assembling.Local;
using Application.FEM._2D;
using Application.FEM._2D.Assembling.Boundaries;
using Application.FEM._2D.Assembling.Global.Sources;
using Application.FEM._2D.Assembling.Local;
using Application.FEM._2D.BasisFunctions;
using Application.FEM._2D.Grid;
using Application.FEM.Assembling._2D.Boundaries.First;
using Application.FEM.Assembling.PortraitBuilders;
using Application.FEM.Core.Assembling.Boundaries.First;
using Application.FEM.Core.Grid.Splitting;
using Domain.Boundaries;
using Domain.Edges;
using Domain.Enums;
using Domain.Environment;
using Domain.Nodes;
using System.Diagnostics;
using System.Globalization;
using System.Numerics;
using Application.EquationSystems.MatrixDecompositions.LU;
using Application.EquationSystems.Preconditions.Separate;
using Application.EquationSystems.Solvers.Sparse;
using Application.FEM._2D.Assembling.Boundaries.First;
using Application.FEM._2D.Assembling.Global;
using Application.FEM.Core.Assembling.Inserters;
using Tests;

Thread.CurrentThread.CurrentCulture = CultureInfo.InvariantCulture;

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

var directProblem = new HarmonicRotorDirectProblem2DFactory().Create();
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