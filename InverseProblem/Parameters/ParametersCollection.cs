using DirectProblem.Core.GridComponents;

namespace InverseProblem.Parameters;

public class ParametersCollection
{
    public Material[] Materials { get; }
    public double[] RControlPoints { get; }
    public double[] ZControlPoints { get; }

    public ParametersCollection(Material[] materials, double[] rControlPoints, double[] zControlPoints)
    {
        Materials = materials;
        RControlPoints = rControlPoints;
        ZControlPoints = zControlPoints;
    }

    public double GetParameterValue(Parameter parameter)
    {
        return parameter.ParameterType switch
        {
            ParameterType.VerticalBound => RControlPoints[parameter.Index],
            ParameterType.HorizontalBound => ZControlPoints[parameter.Index],
            ParameterType.Sigma => Materials[parameter.Index].Sigma,
            _ => throw new ArgumentOutOfRangeException()
        };
    }

    public void SetParameterValue(Parameter parameter, double value)
    {
        switch (parameter.ParameterType)
        {
            case ParameterType.VerticalBound:
                RControlPoints[parameter.Index] = value;
                break;
            case ParameterType.HorizontalBound:
                ZControlPoints[parameter.Index] = value;
                break;
            case ParameterType.Sigma:
                Materials[parameter.Index].Sigma = value;
                break;
            default:
                throw new ArgumentOutOfRangeException();
        }
    }
}