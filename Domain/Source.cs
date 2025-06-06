using System.Numerics;

namespace Domain;

public class Source<TNode, TPower> where TPower : INumber<TPower>
{
    public required TNode Node { get; set; }
    public required TPower Power { get; set; }
}