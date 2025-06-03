using Domain.Nodes;

namespace Domain;

public interface IArea<TNode> where TNode : INode2D
{
    TNode Begin { get; set; }
    TNode End { get; set; }
}

public class Area<TNode> : IArea<TNode> where TNode : INode2D
{
    public required TNode Begin { get; set; }
    public required TNode End { get; set; }
}

public interface IAreaWithMaterial<TNode> : IArea<TNode> where TNode : INode2D
{
    int MaterialId { get; set; }
}

public class AreaWithMaterial<TNode> : IAreaWithMaterial<TNode> where TNode : INode2D
{
    public required TNode Begin { get; set; }
    public required TNode End { get; set; }
    public int MaterialId { get; set; }
}