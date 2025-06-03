namespace Domain.Nodes;

public interface INode
{
    public double X { get; set; }
}

public class Node : INode
{
    public double X { get; set; }
}