using DirectProblem.Core;
using DirectProblem.Core.GridComponents;
using DirectProblem.GridGenerator;
using DirectProblem.GridGenerator.Intervals.Splitting;

namespace DirectProblem;

public class Grids
{
    private static readonly GridBuilder2D GridBuilder = new();

    public static Grid<Node2D> GetUniformGridWith0Dot05Step()
    {
        var grid = GridBuilder
            .SetRAxis(new AxisSplitParameter(
                    new[] { 1e-4, 0.1, 10 },
                    new UniformSplitter(2),
                    new StepProportionalSplitter(0.05, 1.05)
                )
            )
            .SetZAxis(new AxisSplitParameter(
                    new[] { -10d, -6d, -5d, -4d, 0d },
                    new StepProportionalSplitter(0.05, 1/1.05),
                    new StepUniformSplitter(0.05),
                    new StepUniformSplitter(0.05),
                    new StepProportionalSplitter(0.05, 1.05)
                )
            )
            .SetAreas(new Area[]
            {
                new(6, new Node2D(1e-4, -10d), new Node2D(10d, 0d)),
            })
            .Build();

        return grid;
    }

    public static Grid<Node2D> GetUniformGridWith0Dot025Step()
    {
        var grid = GridBuilder
            .SetRAxis(new AxisSplitParameter(
                    new[] { 1e-4, 0.1, 10 },
                    new UniformSplitter(4),
                    new StepProportionalSplitter(0.025, 1.05)
                )
            )
            .SetZAxis(new AxisSplitParameter(
                    new[] { -10d, -6d, -5d, -4d, 0d },
                    new StepProportionalSplitter(0.025, 1/1.05),
                    new StepUniformSplitter(0.025),
                    new StepUniformSplitter(0.025),
                    new StepProportionalSplitter(0.025, 1.05)
                )
            )
            .SetAreas(new Area[]
            {
                new(6, new Node2D(1e-4, -10d), new Node2D(10d, 0d)),
            })
            .Build();

        return grid;
    }

    public static Grid<Node2D> GetUniformGridWith0Dot0125Step()
    {
        var grid = GridBuilder
            .SetRAxis(new AxisSplitParameter(
                    new[] { 1e-4, 0.1, 10 },
                    new UniformSplitter(8),
                    new StepProportionalSplitter(0.0125, 1.05)
                )
            )
            .SetZAxis(new AxisSplitParameter(
                    new[] { -10d, -6d, -5d, -4d, 0d },
                    new StepProportionalSplitter(0.0125, 1/1.05),
                    new StepUniformSplitter(0.0125),
                    new StepUniformSplitter(0.0125),
                    new StepProportionalSplitter(0.0125, 1.05)
                )
            )
            .SetAreas(new Area[]
            {
                new(6, new Node2D(1e-4, -10d), new Node2D(10d, 0d)),
            })
            .Build();

        return grid;
    }

    public static Grid<Node2D> GetUniformGridWith0Dot00625Step()
    {
        var grid = GridBuilder
            .SetRAxis(new AxisSplitParameter(
                    new[] { 1e-4, 0.1, 10 },
                    new UniformSplitter(16),
                    new StepProportionalSplitter(0.00625, 1.05)
                )
            )
            .SetZAxis(new AxisSplitParameter(
                    new[] { -10d, -6d, -5d, -4d, 0d },
                    new StepProportionalSplitter(0.00625, 1/1.05),
                    new StepUniformSplitter(0.00625),
                    new StepUniformSplitter(0.00625),
                    new StepProportionalSplitter(0.00625, 1.05)
                )
            )
            .SetAreas(new Area[]
            {
                new(6, new Node2D(1e-4, -10d), new Node2D(10d, 0d)),
            })
            .Build();

        return grid;
    }

    public static Grid<Node2D> GetUniformGridWith0Dot003125Step()
    {
        var grid = GridBuilder
            .SetRAxis(new AxisSplitParameter(
                    new[] { 1e-4, 0.1, 10 },
                    new UniformSplitter(16),
                    new StepProportionalSplitter(0.003125, 1.05)
                )
            )
            .SetZAxis(new AxisSplitParameter(
                    new[] { -10d, -6d, -5d, -4d, 0d },
                    new StepProportionalSplitter(0.003125, 1/1.05),
                    new StepUniformSplitter(0.003125),
                    new StepUniformSplitter(0.003125),
                    new StepProportionalSplitter(0.003125, 1.05)
                )
            )
            .SetAreas(new Area[]
            {
                new(6, new Node2D(1e-4, -10d), new Node2D(10d, 0d)),
            })
            .Build();

        return grid;
    }

    public static Grid<Node2D> GetUniformGridWith0Dot0015625Step()
    {
        var grid = GridBuilder
            .SetRAxis(new AxisSplitParameter(
                    new[] { 1e-4, 0.1, 10 },
                    new UniformSplitter(32),
                    new StepProportionalSplitter(0.0015625, 1.05)
                )
            )
            .SetZAxis(new AxisSplitParameter(
                    new[] { -10d, -6d, -5d, -4d, 0d },
                    new StepProportionalSplitter(0.0015625, 1/1.05),
                    new StepUniformSplitter(0.0015625),
                    new StepUniformSplitter(0.0015625),
                    new StepProportionalSplitter(0.0015625, 1.05)
                )
            )
            .SetAreas(new Area[]
            {
                new(6, new Node2D(1e-4, -10d), new Node2D(10d, 0d)),
            })
            .Build();

        return grid;
    }

    public static Grid<Node2D> GetUniformSmallGridWith0Dot003125Step()
    {
        var grid = GridBuilder
            .SetRAxis(new AxisSplitParameter(
                    new[] { 1e-4, 0.1, 3d },
                    new UniformSplitter(16),
                    new StepProportionalSplitter(0.003125, 1.1)
                )
            )
            .SetZAxis(new AxisSplitParameter(
                    new[] { -6d, -4d, -3d, -2d, 0d },
                    new StepProportionalSplitter(0.003125, 1/1.1),
                    new StepUniformSplitter(0.003125),
                    new StepUniformSplitter(0.003125),
                    new StepProportionalSplitter(0.003125, 1.1)
                )
            )
            .SetAreas(new Area[]
            {
                new(6, new Node2D(1e-4, -6d), new Node2D(3d, 0d)),
            })
            .Build();

        return grid;
    }

    public static Grid<Node2D> GetSmallGridWith0Dot003125StepWithElementCloseToWell()
    {
        var grid = GridBuilder
            .SetRAxis(new AxisSplitParameter(
                    new[] { 1e-4, 0.1, 1d, 3d },
                    new UniformSplitter(16),
                    new StepProportionalSplitter(0.003125, 1.1),
                    new StepProportionalSplitter(0.1, 1.1)
                )
            )
            .SetZAxis(new AxisSplitParameter(
                    new[] { -6d, -4d, -3d, -2d, 0d },
                    new StepProportionalSplitter(0.003125, 1/1.1),
                    new StepUniformSplitter(0.003125),
                    new StepUniformSplitter(0.003125),
                    new StepProportionalSplitter(0.003125, 1.1)
                )
            )
            .SetAreas(new Area[]
            {
                //скважина
                new(0, new Node2D(1e-4, -6d), new Node2D(0.1, 0d)),
                //первый слой
                new(2, new Node2D(0.1, -2d), new Node2D(10d, 0d)),
                //второй слой
                new(1, new Node2D(0.1, -2.75), new Node2D(1, -2d)),
                new(1, new Node2D(1, -3d), new Node2D(3d, -2d)),
                //искомый элемент
                new(4, new Node2D(0.1, -3.25), new Node2D(1, -2.75)),
                //третий слой
                new(3, new Node2D(0.1, -4d), new Node2D(1, -3.25)),
                new(3, new Node2D(1, -4d), new Node2D(3d, -3d)),
                //четвертый слой
                new(2, new Node2D(0.1, -6d), new Node2D(3d, -4d))
            })
            .Build();

        return grid;
    }

    public static Grid<Node2D> GetSmallGridWith0Dot003125StepWithElementNearToWell()
    {
        var grid = GridBuilder
            .SetRAxis(new AxisSplitParameter(
                    new[] { 1e-4, 0.1, 1d, 2d, 3d },
                    new UniformSplitter(16),
                    new StepProportionalSplitter(0.003125, 1.1),
                    new StepProportionalSplitter(0.1, 1.1),
                    new StepProportionalSplitter(0.25, 1.1)
                )
            )
            .SetZAxis(new AxisSplitParameter(
                    new[] { -6d, -4d, -3d, -2d, 0d },
                    new StepProportionalSplitter(0.003125, 1/1.1),
                    new StepUniformSplitter(0.003125),
                    new StepUniformSplitter(0.003125),
                    new StepProportionalSplitter(0.003125, 1.1)
                )
            )
            .SetAreas(new Area[]
            {
                //скважина
                new(0, new Node2D(1e-4, -6d), new Node2D(0.1, 0d)),
                //первый слой
                new(2, new Node2D(0.1, -2d), new Node2D(10d, 0d)),
                //второй слой
                new(1, new Node2D(0.1, -3), new Node2D(1, -2d)),
                new(1, new Node2D(1, -2.75), new Node2D(2, -2d)),
                new(1, new Node2D(2, -3d), new Node2D(3d, -2d)),
                //искомый элемент
                new(4, new Node2D(1, -3.25), new Node2D(2, -2.75)),
                //третий слой
                new(3, new Node2D(0.1, -4d), new Node2D(1, -3)),
                new(3, new Node2D(1, -4d), new Node2D(2, -3.25)),
                new(3, new Node2D(2, -4d), new Node2D(3d, -3d)),
                //четвертый слой
                new(2, new Node2D(0.1, -6d), new Node2D(3d, -4d))
            })
            .Build();

        return grid;
    }

    public static Grid<Node2D> GetSmallGridWith0Dot003125StepWithElementsCloseToWell()
    {
        var grid = GridBuilder
            .SetRAxis(new AxisSplitParameter(
                    new[] { 1e-4, 0.1, 1d, 3d },
                    new UniformSplitter(16),
                    new StepProportionalSplitter(0.003125, 1.1),
                    new StepProportionalSplitter(0.1, 1.1)
                )
            )
            .SetZAxis(new AxisSplitParameter(
                    new[] { -6d, -4d, -3d, -2d, 0d },
                    new StepProportionalSplitter(0.003125, 1/1.1),
                    new StepUniformSplitter(0.003125),
                    new StepUniformSplitter(0.003125),
                    new StepProportionalSplitter(0.003125, 1.1)
                )
            )
            .SetAreas(new Area[]
            {
                //скважина
                new(0, new Node2D(1e-4, -6d), new Node2D(0.1, 0d)),
                //первый слой
                new(2, new Node2D(0.1, -2d), new Node2D(10d, 0d)),
                //второй слой
                new(1, new Node2D(0.1, -2.25), new Node2D(1, -2d)),
                new(1, new Node2D(0.1, -3), new Node2D(1, -2.75d)),
                new(1, new Node2D(1, -3d), new Node2D(3d, -2d)),
                //искомый элемент
                new(4, new Node2D(0.1, -2.75), new Node2D(1, -2.25)),
                new(4, new Node2D(0.1, -3.75), new Node2D(1, -3.25)),
                //третий слой
                new(3, new Node2D(0.1, -3.25d), new Node2D(1, -3)),
                new(3, new Node2D(0.1, -4d), new Node2D(1, -3.75)),
                new(3, new Node2D(1, -4d), new Node2D(3d, -3d)),
                //четвертый слой
                new(2, new Node2D(0.1, -6d), new Node2D(3d, -4d))
            })
            .Build();

        return grid;
    }

    public static Grid<Node2D> GetSmallGridWith0Dot003125StepWithElementsCloseAndNearToWell()
    {
        var grid = GridBuilder
            .SetRAxis(new AxisSplitParameter(
                    new[] { 1e-4, 0.1, 1d, 2d, 3d },
                    new UniformSplitter(16),
                    new StepProportionalSplitter(0.003125, 1.1),
                    new StepProportionalSplitter(0.1, 1.1),
                    new StepProportionalSplitter(0.25, 1.1)
                )
            )
            .SetZAxis(new AxisSplitParameter(
                    new[] { -6d, -4d, -3d, -2d, 0d },
                    new StepProportionalSplitter(0.003125, 1/1.1),
                    new StepUniformSplitter(0.003125),
                    new StepUniformSplitter(0.003125),
                    new StepProportionalSplitter(0.003125, 1.1)
                )
            )
            .SetAreas(new Area[]
            {
                //скважина
                new(0, new Node2D(1e-4, -6d), new Node2D(0.1, 0d)),
                //первый слой
                new(2, new Node2D(0.1, -2d), new Node2D(10d, 0d)),
                //второй слой
                new(1, new Node2D(0.1, -3), new Node2D(1, -2d)),
                new(1, new Node2D(1, -2.25), new Node2D(2, -2d)),
                new(1, new Node2D(1, -3), new Node2D(2, -2.75d)),
                new(1, new Node2D(2, -3d), new Node2D(3d, -2d)),
                //искомый элемент
                new(4, new Node2D(1, -2.75), new Node2D(2, -2.25)),
                new(4, new Node2D(0.1, -3.75), new Node2D(1, -3.25)),
                //третий слой
                new(3, new Node2D(0.1, -3.25d), new Node2D(1, -3)),
                new(3, new Node2D(0.1, -4d), new Node2D(1, -3.75)),
                new(3, new Node2D(1, -4d), new Node2D(3d, -3d)),
                //четвертый слой
                new(2, new Node2D(0.1, -6d), new Node2D(3d, -4d))
            })
            .Build();

        return grid;
    }

    public static Grid<Node2D> GetGridWith0Dot003125StepWithElementCloseToWell()
    {
        var grid = GridBuilder
            .SetRAxis(new AxisSplitParameter(
                    new[] { 1e-4, 0.1, 1, 10 },
                    new UniformSplitter(32),
                    new StepProportionalSplitter(0.003125, 1.05),
                    new StepProportionalSplitter(0.0021, 1.15)
                )
            )
            .SetZAxis(new AxisSplitParameter(
                    new[] { -10d, -6d, -5d, -4d, 0d },
                    new StepProportionalSplitter(0.003125, 1/1.05),
                    new StepUniformSplitter(0.003125),
                    new StepUniformSplitter(0.003125),
                    new StepProportionalSplitter(0.003125, 1.05)
                )
            )
            .SetAreas(new Area[]
            {
                //скважина
                new(0, new Node2D(1e-4, -10d), new Node2D(0.1, 0d)),
                //первый слой
                new(2, new Node2D(0.1, -4d), new Node2D(10d, 0d)),
                //второй слой
                new(1, new Node2D(0.1, -4.75), new Node2D(1, -4d)),
                new(1, new Node2D(1, -5d), new Node2D(10d, -4d)),
                //искомый элемент
                new(4, new Node2D(0.1, -5.25), new Node2D(1, -4.75)),
                //третий слой
                new(3, new Node2D(0.1, -6d), new Node2D(1, -5.25)),
                new(3, new Node2D(1, -6d), new Node2D(10d, -5d)),
                //четвертый слой
                new(2, new Node2D(0.1, -10d), new Node2D(10d, -6d))
            })
            .Build();

        return grid;
    }

    public static Grid<Node2D> GetGridWith0Dot003125StepWithElementNearToWell()
    {
        var grid = GridBuilder
            .SetRAxis(new AxisSplitParameter(
                    new[] { 1e-4, 0.1, 1, 2, 10 },
                    new UniformSplitter(32),
                    new StepProportionalSplitter(0.003125, 1.05),
                    new StepProportionalSplitter(0.0021, 1.15),
                    new StepProportionalSplitter(0.09, 1.22)
                )
            )
            .SetZAxis(new AxisSplitParameter(
                    new[] { -10d, -6d, -5d, -4d, 0d },
                    new StepProportionalSplitter(0.003125, 1/1.05),
                    new StepUniformSplitter(0.003125),
                    new StepUniformSplitter(0.003125),
                    new StepProportionalSplitter(0.003125, 1.05)
                )
            )
            .SetAreas(new Area[]
            {
                //скважина
                new(0, new Node2D(1e-4, -10d), new Node2D(0.1, 0d)),
                //первый слой
                new(2, new Node2D(0.1, -4d), new Node2D(10d, 0d)),
                //второй слой
                new(1, new Node2D(0.1, -5), new Node2D(1, -4d)),
                new(1, new Node2D(1, -4.75), new Node2D(2, -4d)),
                new(1, new Node2D(2, -5d), new Node2D(10d, -4d)),
                //искомый элемент
                new(4, new Node2D(1, -5.25), new Node2D(2, -4.75)),
                //третий слой
                new(3, new Node2D(0.1, -6d), new Node2D(1, -5)),
                new(3, new Node2D(1, -6d), new Node2D(2, -5.25)),
                new(3, new Node2D(2, -6d), new Node2D(10d, -5d)),
                //четвертый слой
                new(2, new Node2D(0.1, -10d), new Node2D(10d, -6d))
            })
            .Build();

        return grid;
    }

    public static Grid<Node2D> GetGridWith0Dot003125StepWithElementFarFromWell()
    {
        var grid = GridBuilder
            .SetRAxis(new AxisSplitParameter(
                    new[] { 1e-4, 0.1, 5, 6, 10 },
                    new UniformSplitter(32),
                    new StepProportionalSplitter(0.003125, 1.05),
                    new StepProportionalSplitter(0.16, 1.15),
                    new StepProportionalSplitter(0.2, 1.22)
                )
            )
            .SetZAxis(new AxisSplitParameter(
                    new[] { -10d, -6d, -5d, -4d, 0d },
                    new StepProportionalSplitter(0.003125, 1/1.05),
                    new StepUniformSplitter(0.003125),
                    new StepUniformSplitter(0.003125),
                    new StepProportionalSplitter(0.003125, 1.05)
                )
            )
            .SetAreas(new Area[]
            {
                //скважина
                new(0, new Node2D(1e-4, -10d), new Node2D(0.1, 0d)),
                //первый слой
                new(2, new Node2D(0.1, -4d), new Node2D(10d, 0d)),
                //второй слой
                new(1, new Node2D(0.1, -5), new Node2D(5, -4d)),
                new(1, new Node2D(5, -4.75), new Node2D(6, -4d)),
                new(1, new Node2D(6, -5d), new Node2D(10d, -4d)),
                //искомый элемент
                new(4, new Node2D(5, -5.25), new Node2D(6, -4.75)),
                //третий слой
                new(3, new Node2D(0.1, -6d), new Node2D(5, -5)),
                new(3, new Node2D(5, -6d), new Node2D(6, -5.25)),
                new(3, new Node2D(6, -6d), new Node2D(10d, -5d)),
                //четвертый слой
                new(2, new Node2D(0.1, -10d), new Node2D(10d, -6d))
            })
            .Build();

        return grid;
    }

    public static Grid<Node2D> GetGridWith0Dot003125StepWithElementsCloseToWell()
    {
        var grid = GridBuilder
            .SetRAxis(new AxisSplitParameter(
                    new[] { 1e-4, 0.1, 1, 2, 10 },
                    new UniformSplitter(32),
                    new StepProportionalSplitter(0.003125, 1.05),
                    new StepProportionalSplitter(0.0021, 1.15),
                    new StepProportionalSplitter(0.09, 1.22)
                )
            )
            .SetZAxis(new AxisSplitParameter(
                    new[] { -10d, -6d, -5d, -4d, 0d },
                    new StepProportionalSplitter(0.003125, 1/1.05),
                    new StepUniformSplitter(0.003125),
                    new StepUniformSplitter(0.003125),
                    new StepProportionalSplitter(0.003125, 1.05)
                )
            )
            .SetAreas(new Area[]
            {
                //скважина
                new(0, new Node2D(1e-4, -10d), new Node2D(0.1, 0d)),
                //первый слой
                new(2, new Node2D(0.1, -4d), new Node2D(10d, 0d)),
                //второй слой
                new(1, new Node2D(0.1, -4.5), new Node2D(1, -4d)),
                new(1, new Node2D(0.1, -5), new Node2D(1, -4.75d)),
                new(1, new Node2D(1, -5d), new Node2D(10d, -4d)),
                //искомый элемент
                new(4, new Node2D(0.1, -4.75), new Node2D(1, -4.5)),
                //искомый элемент
                new(4, new Node2D(0.1, -5.5), new Node2D(1, -5.25)),
                //третий слой
                new(3, new Node2D(0.1, -5.25d), new Node2D(1, -5)),
                new(3, new Node2D(0.1, -6d), new Node2D(1, -5.5)),
                new(3, new Node2D(1, -6d), new Node2D(10d, -5d)),
                //четвертый слой
                new(2, new Node2D(0.1, -10d), new Node2D(10d, -6d))
            })
            .Build();

        return grid;
    }

    public static Grid<Node2D> GetGridWith0Dot003125StepWithElementsCloseToWellAndNearToWell()
    {
        var grid = GridBuilder
            .SetRAxis(new AxisSplitParameter(
                    new[] { 1e-4, 0.1, 1, 2, 10 },
                    new UniformSplitter(32),
                    new StepProportionalSplitter(0.003125, 1.05),
                    new StepProportionalSplitter(0.0021, 1.15),
                    new StepProportionalSplitter(0.09, 1.22)
                )
            )
            .SetZAxis(new AxisSplitParameter(
                    new[] { -10d, -6d, -5d, -4d, 0d },
                    new StepProportionalSplitter(0.003125, 1/1.05),
                    new StepUniformSplitter(0.003125),
                    new StepUniformSplitter(0.003125),
                    new StepProportionalSplitter(0.003125, 1.05)
                )
            )
            .SetAreas(new Area[]
            {
                //скважина
                new(0, new Node2D(1e-4, -10d), new Node2D(0.1, 0d)),
                //первый слой
                new(2, new Node2D(0.1, -4d), new Node2D(10d, 0d)),
                //второй слой
                new(1, new Node2D(0.1, -4.5), new Node2D(1, -4d)),
                new(1, new Node2D(0.1, -5), new Node2D(1, -4.75d)),
                new(1, new Node2D(1, -5d), new Node2D(10d, -4d)),
                //искомый элемент
                new(4, new Node2D(0.1, -4.75), new Node2D(1, -4.5)),
                //искомый элемент
                new(4, new Node2D(1, -5.5), new Node2D(2, -5.25)),
                //третий слой
                new(3, new Node2D(0.1, -6d), new Node2D(1, -5)),
                new(3, new Node2D(1, -5.25d), new Node2D(2, -5)),
                new(3, new Node2D(1, -6d), new Node2D(2, -5.5)),
                new(3, new Node2D(2, -6d), new Node2D(10d, -5d)),
                //четвертый слой
                new(2, new Node2D(0.1, -10d), new Node2D(10d, -6d))
            })
            .Build();

        return grid;
    }
}