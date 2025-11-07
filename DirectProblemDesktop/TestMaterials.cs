using System.Security.Cryptography;
using Application.FEM.Core.Grid.Splitting;
using Domain.Areas;
using Domain.Materials;

namespace DirectProblemDesktop;

public class TestMaterials
{
    public static MaterialWithSigmaMu[] GetMaterialsForUniformGridWith0Dot003125Step()
    {
        var materials = new[]
        {
            new MaterialWithSigmaMu
            {
                Id = 0,
                Sigma = 1,
            }
        };

        return materials;
    }

    public static MaterialWithSigmaMu[] GetMaterialsUniformGridWith0Dot003125StepWith2Materials()
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
                Id = 0,
                Sigma = 0.05,
            }
        };

        return materials;
    }
}