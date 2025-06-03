using Domain.Nodes;

namespace Domain;

public interface IReceiverLine<TNode> where TNode : INode
{
    TNode ReceiverM { get; set; }
    TNode ReceiverN { get; set; }
}

public class ReceiverLine<TNode> : IReceiverLine<TNode> where TNode : INode
{
    public required TNode ReceiverM { get; set; }
    public required TNode ReceiverN { get; set; }
}