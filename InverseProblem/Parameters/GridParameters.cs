using DirectProblem.Core.GridComponents;
using DirectProblem.GridGenerator.Intervals.Splitting;

namespace InverseProblem.Parameters;

public struct GridParameters
{
    public double[] RControlPoints { get; set; }
    public double[] ZControlPoints { get; set; }
    public IIntervalSplitter[] RSplitters { get; set; }
    public IIntervalSplitter[] ZSplitters { get; set; }
    public Area[] Areas { get; set; }

    public GridParameters
    (
        double[] rControlPoints,
        double[] zControlPoints,
        IIntervalSplitter[] rSplitters,
        IIntervalSplitter[] zSplitters,
        Area[] areas
    )
    {
        RControlPoints = rControlPoints;
        ZControlPoints = zControlPoints;
        RSplitters = rSplitters;
        ZSplitters = zSplitters;
        Areas = areas;
    }
}