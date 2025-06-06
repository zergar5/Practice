namespace Domain.Nodes;

public class Node2D : Node
{
    public double Y { get; set; }
}

public static class Node2DExtensions
{
    public static double R(this Node2D node) => node.X;
    public static double Z(this Node2D node) => node.Y;
}