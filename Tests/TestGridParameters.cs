using Application.FEM._2D.Grid;
using Application.FEM.Core.Grid.Splitting;
using Domain.Areas;

namespace Tests;

public class TestGridParameters
{
    public static GridBuilder2D.Grid2DParameters GetUniformGridWith0Dot003125Step()
    {
        var gridParameters = new GridBuilder2D.Grid2DParameters
        {
            XControlPoints = [1e-4, 0.1, 3d],
            YControlPoints = [-6d, -4d, -3d, -2d, 0d],
            XSplitStrategies =
            [
                new UniformSplitStrategy(16),
                new StepProportionalSplitStrategy(0.003125, 1.1),
            ],
            YSplitStrategies =
            [
                new StepProportionalSplitStrategy(0.003125, 1 / 1.1),
                new StepUniformSplitStrategy(0.003125, true),
                new StepUniformSplitStrategy(0.003125, true),
                new StepProportionalSplitStrategy(0.003125, 1.1),
            ],
            Areas =
            [
                new Area2D
                {
                    MaterialId = 0,
                    XStartControlPointId = 0,
                    XEndControlPointId = 2,
                    YStartControlPointId = 0,
                    YEndControlPointId = 4,
                }
            ]
        };

        return gridParameters;
    }

    public static GridBuilder2D.Grid2DParameters GetGridWith0Dot003125StepWith2Materials()
    {
        var gridParameters = new GridBuilder2D.Grid2DParameters
        {
            XControlPoints = [1e-4, 0.1, 3d],
            YControlPoints = [-6d, -4d, -3d, -2d, 0d],
            XSplitStrategies =
            [
                new UniformSplitStrategy(16),
                new StepProportionalSplitStrategy(0.003125, 1.1),
            ],
            YSplitStrategies =
            [
                new StepProportionalSplitStrategy(0.003125, 1 / 1.1),
                new StepUniformSplitStrategy(0.003125, true),
                new StepUniformSplitStrategy(0.003125, true),
                new StepProportionalSplitStrategy(0.003125, 1.1),
            ],
            Areas =
            [
                //скважина
                new Area2D
                {
                    MaterialId = 0,
                    XStartControlPointId = 0,
                    XEndControlPointId = 1,
                    YStartControlPointId = 0,
                    YEndControlPointId = 4,
                },
                new Area2D
                {
                    MaterialId = 1,
                    XStartControlPointId = 1,
                    XEndControlPointId = 2,
                    YStartControlPointId = 0,
                    YEndControlPointId = 4,
                }
            ]
        };

        return gridParameters;
    }

    public static GridBuilder2D.Grid2DParameters GetGridWith0Dot003125StepWith4Materials()
    {
        var gridParameters = new GridBuilder2D.Grid2DParameters
        {
            XControlPoints = [1e-4, 0.1, 3d],
            YControlPoints = [-6d, -4d, -3d, -2d, 0d],
            XSplitStrategies =
            [
                new UniformSplitStrategy(16),
                new StepProportionalSplitStrategy(0.003125, 1.1),
            ],
            YSplitStrategies =
            [
                new StepProportionalSplitStrategy(0.003125, 1 / 1.1),
                new StepUniformSplitStrategy(0.003125, true),
                new StepUniformSplitStrategy(0.003125, true),
                new StepProportionalSplitStrategy(0.003125, 1.1),
            ],
            Areas =
            [
                //скважина
                new Area2D
                {
                    MaterialId = 0,
                    XStartControlPointId = 0,
                    XEndControlPointId = 1,
                    YStartControlPointId = 0,
                    YEndControlPointId = 4,
                },
                //первый слой
                new Area2D
                {
                    MaterialId = 1,
                    XStartControlPointId = 1,
                    XEndControlPointId = 2,
                    YStartControlPointId = 3,
                    YEndControlPointId = 4,
                },
                //второй слой
                new Area2D
                {
                    MaterialId = 2,
                    XStartControlPointId = 1,
                    XEndControlPointId = 2,
                    YStartControlPointId = 1,
                    YEndControlPointId = 3,
                },
                //третий слой
                new Area2D
                {
                    MaterialId = 3,
                    XStartControlPointId = 1,
                    XEndControlPointId = 2,
                    YStartControlPointId = 0,
                    YEndControlPointId = 1,
                }
            ]
        };

        return gridParameters;
    }


    public static GridBuilder2D.Grid2DParameters GetGridWith0Dot003125StepWithElementCloseToWellWith8Materials()
    {
        var gridParameters = new GridBuilder2D.Grid2DParameters
        {
            XControlPoints = [1e-4, 0.1, 1d, 3d],
            YControlPoints = [-6d, -4d, -3.25, -3d, -2.75, -2d, 0d],
            XSplitStrategies =
            [
                new UniformSplitStrategy(16),
                new StepProportionalSplitStrategy(0.003125, 1.1),
                new StepProportionalSplitStrategy(0.1, 1.1),
            ],
            YSplitStrategies =
            [
                new StepProportionalSplitStrategy(0.003125, 1 / 1.1),
                new StepUniformSplitStrategy(0.003125, true),
                new StepUniformSplitStrategy(0.003125, true),
                new StepUniformSplitStrategy(0.003125, true),
                new StepUniformSplitStrategy(0.003125, true),
                new StepProportionalSplitStrategy(0.003125, 1.1),
            ],
            Areas =
            [
                //скважина
                new Area2D
                {
                    MaterialId = 0,
                    XStartControlPointId = 0,
                    XEndControlPointId = 1,
                    YStartControlPointId = 0,
                    YEndControlPointId = 6,
                },
                //первый слой
                new Area2D
                {
                    MaterialId = 1,
                    XStartControlPointId = 1,
                    XEndControlPointId = 3,
                    YStartControlPointId = 5,
                    YEndControlPointId = 6,
                },
                //второй слой
                new Area2D
                {
                    MaterialId = 2,
                    XStartControlPointId = 1,
                    XEndControlPointId = 2,
                    YStartControlPointId = 4,
                    YEndControlPointId = 5,
                },
                new Area2D
                {
                    MaterialId = 3,
                    XStartControlPointId = 2,
                    XEndControlPointId = 3,
                    YStartControlPointId = 3,
                    YEndControlPointId = 5,
                },
                //искомый элемент
                new Area2D
                {
                    MaterialId = 4,
                    XStartControlPointId = 1,
                    XEndControlPointId = 2,
                    YStartControlPointId = 2,
                    YEndControlPointId = 4,
                },
                //третий слой
                new Area2D
                {
                    MaterialId = 5,
                    XStartControlPointId = 1,
                    XEndControlPointId = 2,
                    YStartControlPointId = 1,
                    YEndControlPointId = 2,
                },
                new Area2D
                {
                    MaterialId = 6,
                    XStartControlPointId = 2,
                    XEndControlPointId = 3,
                    YStartControlPointId = 1,
                    YEndControlPointId = 3,
                },
                //четвертый слой
                new Area2D
                {
                    MaterialId = 7,
                    XStartControlPointId = 1,
                    XEndControlPointId = 3,
                    YStartControlPointId = 0,
                    YEndControlPointId = 1,
                },
            ]
        };

        return gridParameters;
    }

    public static GridBuilder2D.Grid2DParameters GetGridWith0Dot003125StepWithElementNearToWellWith8Materials()
    {
        var gridParameters = new GridBuilder2D.Grid2DParameters
        {
            XControlPoints = [1e-4, 0.1, 1d, 2d, 3d],
            YControlPoints = [-6d, -4d, -3.25, -3d, -2.75, -2d, 0d],
            XSplitStrategies =
            [
                new UniformSplitStrategy(16),
                new StepProportionalSplitStrategy(0.003125, 1.1),
                new StepProportionalSplitStrategy(0.1, 1.1),
                new StepProportionalSplitStrategy(0.25, 1.1)
            ],
            YSplitStrategies =
            [
                new StepProportionalSplitStrategy(0.003125, 1 / 1.1),
                new StepUniformSplitStrategy(0.003125, true),
                new StepUniformSplitStrategy(0.003125, true),
                new StepUniformSplitStrategy(0.003125, true),
                new StepUniformSplitStrategy(0.003125, true),
                new StepProportionalSplitStrategy(0.003125, 1.1),
            ],
            Areas =
            [
                //скважина
                new Area2D
                {
                    MaterialId = 0,
                    XStartControlPointId = 0,
                    XEndControlPointId = 1,
                    YStartControlPointId = 0,
                    YEndControlPointId = 6,
                },
                //первый слой
                new Area2D
                {
                    MaterialId = 1,
                    XStartControlPointId = 1,
                    XEndControlPointId = 4,
                    YStartControlPointId = 5,
                    YEndControlPointId = 6,
                },
                //второй слой
                new Area2D
                {
                    MaterialId = 2,
                    XStartControlPointId = 1,
                    XEndControlPointId = 2,
                    YStartControlPointId = 3,
                    YEndControlPointId = 5,
                },
                new Area2D
                {
                    MaterialId = 3,
                    XStartControlPointId = 2,
                    XEndControlPointId = 3,
                    YStartControlPointId = 4,
                    YEndControlPointId = 5,
                },
                new Area2D
                {
                    MaterialId = 3,
                    XStartControlPointId = 3,
                    XEndControlPointId = 4,
                    YStartControlPointId = 3,
                    YEndControlPointId = 5,
                },
                //искомый элемент
                new Area2D
                {
                    MaterialId = 4,
                    XStartControlPointId = 2,
                    XEndControlPointId = 3,
                    YStartControlPointId = 2,
                    YEndControlPointId = 4,
                },
                //третий слой
                new Area2D
                {
                    MaterialId = 5,
                    XStartControlPointId = 1,
                    XEndControlPointId = 2,
                    YStartControlPointId = 1,
                    YEndControlPointId = 3,
                },
                new Area2D
                {
                    MaterialId = 6,
                    XStartControlPointId = 2,
                    XEndControlPointId = 3,
                    YStartControlPointId = 1,
                    YEndControlPointId = 2,
                },
                new Area2D
                {
                    MaterialId = 6,
                    XStartControlPointId = 3,
                    XEndControlPointId = 4,
                    YStartControlPointId = 1,
                    YEndControlPointId = 3,
                },
                //четвертый слой
                new Area2D
                {
                    MaterialId = 7,
                    XStartControlPointId = 1,
                    XEndControlPointId = 4,
                    YStartControlPointId = 0,
                    YEndControlPointId = 1,
                },
            ]
        };

        return gridParameters;
    }

    public static GridBuilder2D.Grid2DParameters GetGridWith0Dot003125StepWithElementsCloseAndNearToWellWith8Materials()
    {
        var gridParameters = new GridBuilder2D.Grid2DParameters
        {
            XControlPoints = [1e-4, 0.1, 1d, 2d, 3d],
            YControlPoints = [-6d, -4d, -3.75, -3.25, -3d, -2.75, -2.25, -2d, 0d],
            XSplitStrategies =
            [
                new UniformSplitStrategy(16),
                new StepProportionalSplitStrategy(0.003125, 1.1),
                new StepProportionalSplitStrategy(0.1, 1.1),
            ],
            YSplitStrategies =
            [
                new StepProportionalSplitStrategy(0.003125, 1 / 1.1),
                new StepUniformSplitStrategy(0.003125, true),
                new StepUniformSplitStrategy(0.003125, true),
                new StepUniformSplitStrategy(0.003125, true),
                new StepUniformSplitStrategy(0.003125, true),
                new StepUniformSplitStrategy(0.003125, true),
                new StepUniformSplitStrategy(0.003125, true),
                new StepProportionalSplitStrategy(0.003125, 1.1),
            ],
            Areas =
            [
                //скважина
                new Area2D
                {
                    MaterialId = 0,
                    XStartControlPointId = 0,
                    XEndControlPointId = 1,
                    YStartControlPointId = 0,
                    YEndControlPointId = 8,
                },
                //первый слой
                new Area2D
                {
                    MaterialId = 1,
                    XStartControlPointId = 1,
                    XEndControlPointId = 3,
                    YStartControlPointId = 7,
                    YEndControlPointId = 8,
                },
                //второй слой
                new Area2D
                {
                    MaterialId = 2,
                    XStartControlPointId = 1,
                    XEndControlPointId = 2,
                    YStartControlPointId = 4,
                    YEndControlPointId = 7,
                },
                new Area2D
                {
                    MaterialId = 3,
                    XStartControlPointId = 2,
                    XEndControlPointId = 3,
                    YStartControlPointId = 6,
                    YEndControlPointId = 7,
                },
                new Area2D
                {
                    MaterialId = 3,
                    XStartControlPointId = 2,
                    XEndControlPointId = 3,
                    YStartControlPointId = 4,
                    YEndControlPointId = 5,
                },
                new Area2D
                {
                    MaterialId = 3,
                    XStartControlPointId = 3,
                    XEndControlPointId = 4,
                    YStartControlPointId = 4,
                    YEndControlPointId = 7,
                },
                //искомый элемент
                new Area2D
                {
                    MaterialId = 4,
                    XStartControlPointId = 2,
                    XEndControlPointId = 3,
                    YStartControlPointId = 5,
                    YEndControlPointId = 6,
                },
                new Area2D
                {
                    MaterialId = 4,
                    XStartControlPointId = 2,
                    XEndControlPointId = 3,
                    YStartControlPointId = 2,
                    YEndControlPointId = 3,
                },
                //третий слой
                new Area2D
                {
                    MaterialId = 5,
                    XStartControlPointId = 1,
                    XEndControlPointId = 2,
                    YStartControlPointId = 3,
                    YEndControlPointId = 4,
                },
                new Area2D
                {
                    MaterialId = 5,
                    XStartControlPointId = 1,
                    XEndControlPointId = 2,
                    YStartControlPointId = 1,
                    YEndControlPointId = 2,
                },
                new Area2D
                {
                    MaterialId = 6,
                    XStartControlPointId = 2,
                    XEndControlPointId = 4,
                    YStartControlPointId = 1,
                    YEndControlPointId = 4,
                },
                //четвертый слой
                new Area2D
                {
                    MaterialId = 7,
                    XStartControlPointId = 1,
                    XEndControlPointId = 3,
                    YStartControlPointId = 0,
                    YEndControlPointId = 1,
                },
            ]
        };

        return gridParameters;
    }
}