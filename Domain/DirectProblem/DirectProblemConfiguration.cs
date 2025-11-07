using Domain.Enums;

namespace Domain.DirectProblem;

public class DirectProblemConfiguration
{
    public Dimension Dimension { get; set; }
    public CoordinateSystem CoordinateSystem { get; set; }
    public bool? IsRotor { get; set; }
    public bool? IsHarmonic { get; set; }
}