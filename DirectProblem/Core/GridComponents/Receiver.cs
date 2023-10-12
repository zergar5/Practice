namespace DirectProblem.Core.GridComponents;

public class Receiver
{
    public Node2D Point { get; init; }
    public double Potential { get; set; } = 0;

    public Receiver(Node2D point)
    {
        Point = point;
    }
}