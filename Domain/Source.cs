using Domain.Nodes;

namespace Domain;

public interface ISource<TNode> where TNode : INode
{
    TNode Node { get; set; }
    double Power { get; set; }
}

public class Source<TNode> : ISource<TNode> where TNode : INode
{
    public required TNode Node { get; set; }
    public double Power { get; set; }
}