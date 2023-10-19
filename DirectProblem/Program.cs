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
            new[] { 1e-3, 0.1, 1 },
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
        new(0, new Node2D(1e-3, -12d), new Node2D(0.1, 0d)),
        //первый слой
        new(1, new Node2D(0.1, -3d), new Node2D(0.75, 0d)),
        new(2, new Node2D(0.75, -3d), new Node2D(1d, 0d)),
        //второй слой
        new(3, new Node2D(0.1, -6d), new Node2D(0.35, -3d)),
        new(4, new Node2D(0.35, -6d), new Node2D(1d, -3d)),
        //третий слой
        new(4, new Node2D(0.1, -9d), new Node2D(0.65, -6d)),
        new(3, new Node2D(0.65, -9d), new Node2D(1d, -6d)),
        //четвертый слой
        new(2, new Node2D(0.1, -12d), new Node2D(0.25, -9d)),
        new(1, new Node2D(0.25, -12d), new Node2D(1d, -9d)),
    })
    //.SetAreas(new Area[]
    //{
    //    //скважина
    //    new(0, new Node2D(1e-3, -12d), new Node2D(0.1, 0d)),
    //    //первый слой
    //    new(2, new Node2D(0.1, -3d), new Node2D(0.75, 0d)),
    //    new(2, new Node2D(0.75, -3d), new Node2D(1d, 0d)),
    //    //второй слой
    //    new(2, new Node2D(0.1, -6d), new Node2D(0.5, -3d)),
    //    new(3, new Node2D(0.5, -6d), new Node2D(0.6, -3d)),
    //    new(2, new Node2D(0.6, -6d), new Node2D(1d, -3d)),
    //    //третий слой
    //    new(2, new Node2D(0.1, -9d), new Node2D(0.65, -6d)),
    //    new(2, new Node2D(0.65, -9d), new Node2D(1d, -6d)),
    //    //четвертый слой
    //    new(2, new Node2D(0.1, -12d), new Node2D(0.25, -9d)),
    //    new(2, new Node2D(0.25, -12d), new Node2D(1d, -9d)),
    //})
    .Build();

var materialFactory = new MaterialFactory
(
    new List<double> { 1d, 1d, 1d, 1d, 1d, 1d },
    new List<double> { 0.5, 0.1, 0.05, 1d/3d, 0.2, 0d }
);

var omegas = new[] { 4e4, 2e5, 1e6, 2e6 };
var current = 10000d;

var sources = new FocusedSource[]
{
    new(new Node2D(0.0495d, -1d), current),
    new(new Node2D(0.0495d, -2d), current), 
    new(new Node2D(0.0495d, -4d), current),
    new(new Node2D(0.0495d, -5d), current), 
    new(new Node2D(0.0495d, -7d), current),
    new(new Node2D(0.0495d, -8d), current)
};

var firstBoundaryProvider = new FirstBoundaryProvider(grid);
var conditions = firstBoundaryProvider.GetConditions(20, 40);

var points = new Node2D[119];

for (var i = 1; i <= 119; i++)
{
    points[i-1] = new Node2D(0.0495d, -0.1 * i);
}

var directProblemSolver = new DirectProblemSolver(grid, materialFactory, conditions, points);

var resultO = new ResultIO("../DirectProblem/Results/");

for (var i = 0; i < sources.Length; i++)
{
    for (var j = 0; j < omegas.Length; j++)
    {
        var result = directProblemSolver
            .SetOmega(omegas[j])
            .SetSource(sources[i])
            .AssembleSLAE()
            .Solve();

        resultO.Write($"omega{j} source{i}.txt", omegas[j], points, result);
    }
}