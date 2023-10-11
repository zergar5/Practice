﻿using DirectProblem.Calculus;
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
//var grid = gridBuilder2D
//    .SetRAxis(new AxisSplitParameter(
//            new[] { 1d, 1.1d, 1.101d, 15d },
//            new UniformSplitter(2),
//            new UniformSplitter(1),
//            new UniformSplitter(7)
//        )
//    )
//    .SetZAxis(new AxisSplitParameter(
//            new[] { -13d, -10d, -7d, -4d, -1d },
//            new ProportionalSplitter(4, 0.7),
//            new ProportionalSplitter(16, 0.9),
//            new ProportionalSplitter(16, 1.1),
//            new ProportionalSplitter(4, 1.3)
//        )
//    )
//    .SetMaterials(new[]
//    {
//        0, 0, 1, 2, 2, 2, 2, 2, 2, 2, //0
//        0, 0, 1, 2, 2, 2, 2, 2, 2, 2,
//        0, 0, 1, 2, 2, 2, 2, 2, 2, 2,
//        0, 0, 1, 2, 2, 2, 2, 2, 2, 2, //3
//        0, 0, 1, 3, 3, 4, 4, 4, 4, 4, //0
//        0, 0, 1, 3, 3, 4, 4, 4, 4, 4,
//        0, 0, 1, 3, 3, 4, 4, 4, 4, 4,
//        0, 0, 1, 3, 3, 4, 4, 4, 4, 4,
//        0, 0, 1, 3, 3, 4, 4, 4, 4, 4,
//        0, 0, 1, 3, 3, 4, 4, 4, 4, 4,
//        0, 0, 1, 3, 3, 4, 4, 4, 4, 4,
//        0, 0, 1, 3, 3, 4, 4, 4, 4, 4,
//        0, 0, 1, 3, 3, 4, 4, 4, 4, 4,
//        0, 0, 1, 3, 3, 4, 4, 4, 4, 4,
//        0, 0, 1, 3, 3, 4, 4, 4, 4, 4,
//        0, 0, 1, 3, 3, 4, 4, 4, 4, 4,
//        0, 0, 1, 3, 3, 4, 4, 4, 4, 4,
//        0, 0, 1, 3, 3, 4, 4, 4, 4, 4,
//        0, 0, 1, 3, 3, 4, 4, 4, 4, 4,
//        0, 0, 1, 3, 3, 4, 4, 4, 4, 4, // 15
//        0, 0, 1, 4, 4, 3, 3, 3, 3, 3, //0
//        0, 0, 1, 4, 4, 3, 3, 3, 3, 3,
//        0, 0, 1, 4, 4, 3, 3, 3, 3, 3,
//        0, 0, 1, 4, 4, 3, 3, 3, 3, 3,
//        0, 0, 1, 4, 4, 3, 3, 3, 3, 3,
//        0, 0, 1, 4, 4, 3, 3, 3, 3, 3,
//        0, 0, 1, 4, 4, 3, 3, 3, 3, 3,
//        0, 0, 1, 4, 4, 3, 3, 3, 3, 3,
//        0, 0, 1, 4, 4, 3, 3, 3, 3, 3,
//        0, 0, 1, 4, 4, 3, 3, 3, 3, 3,
//        0, 0, 1, 4, 4, 3, 3, 3, 3, 3,
//        0, 0, 1, 4, 4, 3, 3, 3, 3, 3,
//        0, 0, 1, 4, 4, 3, 3, 3, 3, 3,
//        0, 0, 1, 4, 4, 3, 3, 3, 3, 3,
//        0, 0, 1, 4, 4, 3, 3, 3, 3, 3,
//        0, 0, 1, 4, 4, 3, 3, 3, 3, 3, // 15
//        0, 0, 1, 2, 2, 2, 2, 2, 2, 2, //0
//        0, 0, 1, 2, 2, 2, 2, 2, 2, 2,
//        0, 0, 1, 2, 2, 2, 2, 2, 2, 2,
//        0, 0, 1, 2, 2, 2, 2, 2, 2, 2, //3
//    })
//    .Build();

var grid = gridBuilder2D
    .SetRAxis(new AxisSplitParameter(
            new[] { 1d, 3d },
            new UniformSplitter(2)
        )
    )
    .SetZAxis(new AxisSplitParameter(
            new[] { 1d, 3d },
            new UniformSplitter(2)
        )
    )
    .Build();

var materialFactory = new MaterialFactory
(
    new List<double> { 1d, 1d, 1d, 1d, 1d },
    new List<double> { 1d, 0.9, 0.5, 0.1, 0.25 }
);
var omega = 100000d;

var localBasisFunctionsProvider = new LocalBasisFunctionsProvider(grid, new LinearFunctionsProvider());

Func<Node2D, Complex> u = p => new Complex((p.R - 1d) * (p.R - 15d) * (p.Z + 1d) * (p.Z + 13d), -(p.R - 1d) * (p.R - 15d) * (p.Z + 1d) * (p.Z + 13d));

//var f = new RightPartParameter
//(
//    (p, mu, sigma) => new Complex(
//        (-((p.Z + 13d) * (p.Z + 1d) * (4d * p.R - 16d) / p.R) + u(p).Real / (p.R * p.R) -
//         2d * (p.R - 15d) * (p.R - 1d)) / mu - omega * sigma * u(p).Imaginary,
//        ((p.Z + 13d) * (p.Z + 1d) * (4d * p.R - 16d) / p.R + u(p).Imaginary / (p.R * p.R) +
//         2d * (p.R - 15d) * (p.R - 1d)) / mu + omega * sigma * u(p).Real
//    ),
//    grid
//);

//Func<Node2D, Complex> u = p => new Complex(Exp(p.R), Exp(p.Z));

//var f = new RightPartParameter
//(
//    (p, mu, sigma) => new Complex(
//        (-Exp(p.R) * (p.R + 1) / p.R + u(p).Real / Pow(p.R, 2)) / mu - omega * sigma * u(p).Imaginary,
//        (-Exp(p.Z) + u(p).Imaginary / Pow(p.R, 2)) / mu + omega * sigma * u(p).Real
//    ),
//    grid
//);

//Func<Node2D, Complex> u = p => new Complex(Pow(p.R, 2), Pow(p.Z, 2));

//var f = new RightPartParameter
//(
//    (p, mu, sigma) => new Complex(
//        -3d - omega * sigma * u(p).Imaginary,
//        -2d / mu + u(p).Imaginary / (Pow(p.R, 2) * mu) + omega * sigma * u(p).Real
//    ),
//    grid
//);

//Func<Node2D, Complex> u = p => new Complex(Pow(p.R, 2), Pow(p.R, 2));

//var f = new RightPartParameter
//(
//    (p, mu, sigma) => new Complex(
//        -3d - omega * sigma * u(p).Imaginary,
//        -3d + omega * sigma * u(p).Real
//    ),
//    grid
//);

//Func<Node2D, Complex> u = p => new Complex(p.R, p.Z);

//var f = new RightPartParameter
//(
//    (p, mu, sigma) => new Complex(
//        -1d / (mu * p.R) + u(p).Real / (Pow(p.R, 2) * mu) - omega * sigma * u(p).Imaginary,
//        u(p).Imaginary / (Pow(p.R, 2) * mu) + omega * sigma * u(p).Real
//    ),
//    grid
//);

//Func<Node2D, Complex> u = p => new Complex(p.R, p.R);

//var f = new RightPartParameter
//(
//    (p, mu, sigma) => new Complex(
//        -omega * sigma * u(p).Imaginary,
//        omega * sigma * u(p).Real
//    ),
//    grid
//);

var derivativeCalculator = new DerivativeCalculator();

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
    .ApplyFirstConditions(conditions)
    .BuildEquation();

new Vector(new double[] { 1d, 1d, 2d, 2d, 3d, 3d, 1d, 1d, 0d, 0d, 3d, 3d, 1d, 1d, 2d, 2d, 3d, 3d }).Copy(equation.RightPart);

var preconditionMatrix = globalAssembler.AllocatePreconditionMatrix();

var luPreconditioner = new LUPreconditioner();

var los = new LOS(luPreconditioner, new LUSparse(), preconditionMatrix);
var solution = los.Solve(equation);

var femSolution = new FEMSolution(grid, solution, localBasisFunctionsProvider, omega);

var fieldValues = femSolution.CalculateField(5d, 1.05d);

foreach (var value in fieldValues)
{
    Console.WriteLine(value);
}