using DirectProblem.Core;
using DirectProblem.Core.GridComponents;
using DirectProblem.GridGenerator;
using DirectProblem.GridGenerator.Intervals.Splitting;

namespace DirectProblem;

public class Grids
{
    private static readonly GridBuilder2D GridBuilder = new();

    public static Grid<Node2D> GetUniformGridWith0Dot003125Step()
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
                    new StepProportionalSplitter(0.003125, 1 / 1.1),
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

    public static Grid<Node2D> GetUniformGridWith0Dot003125StepWith2Materials()
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
                    new StepProportionalSplitter(0.003125, 1 / 1.1),
                    new StepUniformSplitter(0.003125),
                    new StepUniformSplitter(0.003125),
                    new StepProportionalSplitter(0.003125, 1.1)
                )
            )
            .SetAreas(new Area[]
            {
                new(0, new Node2D(1e-4, -6d), new Node2D(0.1, 0d)),
                new(3, new Node2D(0.1, -6d), new Node2D(3, 0d))
            })
            .Build();

        return grid;
    }

    public static Grid<Node2D> GetGridWith0Dot003125StepWithElementCloseToWell()
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
                    new StepProportionalSplitter(0.003125, 1 / 1.1),
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
                new(2, new Node2D(0.1, -2d), new Node2D(3d, 0d)),
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

    public static Grid<Node2D> GetGridWith0Dot003125StepWithElementNearToWell()
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
                    new StepProportionalSplitter(0.003125, 1 / 1.1),
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

    public static Grid<Node2D> GetGridWith0Dot003125StepWithElementsCloseToWell()
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
                    new StepProportionalSplitter(0.003125, 1 / 1.1),
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

    public static Grid<Node2D> GetGridWith0Dot003125StepWithElementsCloseAndNearToWell()
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
                    new StepProportionalSplitter(0.003125, 1 / 1.1),
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

    public static Grid<Node2D> GetGridWith0Dot003125StepWithElementCloseToWellAnd8Sigmas()
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
                    new StepProportionalSplitter(0.003125, 1 / 1.1),
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
                new(1, new Node2D(0.1, -2d), new Node2D(3d, 0d)),
                //второй слой
                new(2, new Node2D(0.1, -2.75), new Node2D(1, -2d)),
                new(3, new Node2D(1, -3d), new Node2D(3d, -2d)),
                //искомый элемент
                new(4, new Node2D(0.1, -3.25), new Node2D(1, -2.75)),
                //третий слой
                new(5, new Node2D(0.1, -4d), new Node2D(1, -3.25)),
                new(6, new Node2D(1, -4d), new Node2D(3d, -3d)),
                //четвертый слой
                new(7, new Node2D(0.1, -6d), new Node2D(3d, -4d))
            })
            .Build();

        return grid;
    }

    public static Grid<Node2D> GetGridWith0Dot003125StepWithElementNearToWellAnd8Sigmas()
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
                    new StepProportionalSplitter(0.003125, 1 / 1.1),
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
                new(1, new Node2D(0.1, -2d), new Node2D(3d, 0d)),
                //второй слой
                new(2, new Node2D(0.1, -3), new Node2D(1, -2d)),
                new(3, new Node2D(1, -2.75), new Node2D(2, -2d)),
                new(3, new Node2D(2, -3d), new Node2D(3d, -2d)),
                //искомый элемент
                new(4, new Node2D(1, -3.25), new Node2D(2, -2.75)),
                //третий слой
                new(5, new Node2D(0.1, -4d), new Node2D(1, -3)),
                new(6, new Node2D(1, -4d), new Node2D(2, -3.25)),
                new(6, new Node2D(2, -4d), new Node2D(3d, -3d)),
                //четвертый слой
                new(7, new Node2D(0.1, -6d), new Node2D(3d, -4d))
            })
            .Build();

        return grid;
    }

    public static Grid<Node2D> GetGridWith0Dot003125StepWithElementsCloseAndNearToWellAnd8Sigmas()
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
                    new StepProportionalSplitter(0.003125, 1 / 1.1),
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
                new(1, new Node2D(0.1, -2d), new Node2D(3d, 0d)),
                //второй слой
                new(2, new Node2D(0.1, -3), new Node2D(1, -2d)),
                new(3, new Node2D(1, -2.25), new Node2D(2, -2d)),
                new(3, new Node2D(1, -3), new Node2D(2, -2.75d)),
                new(3, new Node2D(2, -3d), new Node2D(3d, -2d)),
                //искомый элемент
                new(4, new Node2D(1, -2.75), new Node2D(2, -2.25)),
                new(4, new Node2D(0.1, -3.75), new Node2D(1, -3.25)),
                //третий слой
                new(5, new Node2D(0.1, -3.25d), new Node2D(1, -3)),
                new(5, new Node2D(0.1, -4d), new Node2D(1, -3.75)),
                new(6, new Node2D(1, -4d), new Node2D(3d, -3d)),
                //четвертый слой
                new(7, new Node2D(0.1, -6d), new Node2D(3d, -4d))
            })
            .Build();

        return grid;
    }
}