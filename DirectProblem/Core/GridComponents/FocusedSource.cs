namespace DirectProblem.Core.GridComponents;

public class FocusedSource
{
    public Node2D Point { get; init; }
    public double Current { get; set; } = 0;
    public double Potential { get; set; } = 0;

    public FocusedSource(Node2D point)
    {
        Point = point;
    }

    public FocusedSource(Node2D point, double current)
    {
        Point = point;
        Current = current;
    }
};