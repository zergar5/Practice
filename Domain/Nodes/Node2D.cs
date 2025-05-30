namespace Domain.Nodes;

public interface INode2D
{
    double X { get; set; }
    double Y { get; set; }
}

public class Node2D : INode2D
{
    public double X { get; set; }
    public double Y { get; set; }
}