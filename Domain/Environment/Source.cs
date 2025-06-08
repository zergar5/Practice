using System.Numerics;

namespace Domain.Environment;

public class Source<TNode>
{
    public required TNode Node { get; set; }
    public required double Power { get; set; }
}