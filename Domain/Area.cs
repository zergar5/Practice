using Domain.Nodes;

namespace Domain;

public interface IArea<TNode> where TNode : INode2D
{
    TNode LowerLeftCorner { get; set; }
    TNode UpperRightCorner { get; set; }
}

public class Area<TNode> : IArea<TNode> where TNode : INode2D
{
    public required TNode LowerLeftCorner { get; set; }
    public required TNode UpperRightCorner { get; set; }
}

public interface IAreaWithMaterial<TNode> : IArea<TNode> where TNode : INode2D
{
    int MaterialId { get; set; }
}

public class AreaWithMaterial<TNode> : IAreaWithMaterial<TNode> where TNode : INode2D
{
    public required TNode LowerLeftCorner { get; set; }
    public required TNode UpperRightCorner { get; set; }
    public int MaterialId { get; set; }
    
}