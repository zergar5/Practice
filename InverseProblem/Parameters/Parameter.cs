namespace InverseProblem.Parameters;

public enum ParameterType
{
    Sigma,
    VerticalBound,
    HorizontalBound
}

public readonly record struct Parameter(ParameterType ParameterType, int Index, double Value);