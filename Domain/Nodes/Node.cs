namespace Domain.Nodes;

public class Node
{
    public double X { get; set; }
}

public static class NodeExtensions
{
    public static double R(this Node node) => node.X;
}