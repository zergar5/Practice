namespace Application.InverseProblem.Parameters;

public class Parameter
{
    public ParameterType Type { get; set; }
    public int Index { get; set; }
    public double InitialValue { get; set; }
}

public enum ParameterType
{
    Sigma,
    VerticalBound,
    HorizontalBound
}