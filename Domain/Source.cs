using Domain.Nodes;

namespace Domain;

public interface ISource<TNode> where TNode : INode2D
{
    TNode Node { get; set; }
    double Power { get; set; }
}

public class Source<TNode> : ISource<TNode> where TNode : INode2D
{
    public required TNode Node { get; set; }
    public double Power { get; set; }
}