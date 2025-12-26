using Domain.Materials;

namespace Tests;

public class TestMaterials
{
    public static MaterialWithSigmaMu[] GetMaterialsForUniformGridWith0Dot003125Step()
    {
        var materials = new[]
        {
            new MaterialWithSigmaMu
            {
                Id = 0,
                Sigma = 0.5,
            }
        };

        return materials;
    }

    public static MaterialWithSigmaMu[] GetMaterialsForGridWith0Dot003125StepWith2Materials()
    {
        var materials = new[]
        {
            new MaterialWithSigmaMu
            {
                Id = 0,
                Sigma = 0.5,
            },
            new MaterialWithSigmaMu
            {
                Id = 1,
                Sigma = 0.05,
            }
        };

        return materials;
    }

    public static MaterialWithSigmaMu[] GetMaterialsForGridWith0Dot003125StepWith4Materials()
    {
        var materials = new[]
        {
            new MaterialWithSigmaMu
            {
                Id = 0,
                Sigma = 0.5,
            },
            new MaterialWithSigmaMu
            {
                Id = 1,
                Sigma = 0.05,
            },
            new MaterialWithSigmaMu
            {
                Id = 2,
                Sigma = 0.1,
            },
            new MaterialWithSigmaMu
            {
                Id = 3,
                Sigma = 0.25,
            }
        };

        return materials;
    }

    public static MaterialWithSigmaMu[] GetMaterialsForGridWith0Dot003125StepWith8Materials()
    {
        var materials = new[]
        {
            new MaterialWithSigmaMu
            {
                Id = 0,
                Sigma = 0.5,
            },
            new MaterialWithSigmaMu
            {
                Id = 1,
                Sigma = 0.05,
            },
            new MaterialWithSigmaMu
            {
                Id = 2,
                Sigma = 1d / 30,
            },
            new MaterialWithSigmaMu
            {
                Id = 3,
                Sigma = 0.01,
            },
            new MaterialWithSigmaMu
            {
                Id = 4,
                Sigma = 1d / 3,
            },
            new MaterialWithSigmaMu
            {
                Id = 5,
                Sigma = 0.2,
            },
            new MaterialWithSigmaMu
            {
                Id = 6,
                Sigma = 0.1,
            },
            new MaterialWithSigmaMu
            {
                Id = 7,
                Sigma = 0.25,
            },
        };

        return materials;
    }
}