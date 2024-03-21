using DirectProblem.Core;
using DirectProblem.Core.GridComponents;
using DirectProblem.GridGenerator;
using DirectProblem.GridGenerator.Intervals.Splitting;

namespace DirectProblem;

public class Grids
{
    private static readonly GridBuilder2D GridBuilder = new GridBuilder2D();

    public static Grid<Node2D> GetUniformGridWith0Dot025Step()
    {
        var grid = GridBuilder
            .SetRAxis(new AxisSplitParameter(
                    new[] { 1e-4, 0.1, 1, 10 },
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
                    new[] { 1e-4, 0.1, 1, 10 },
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
                    new[] { 1e-4, 0.1, 1, 10 },
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
                    new[] { 1e-4, 0.1, 1, 10 },
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
}