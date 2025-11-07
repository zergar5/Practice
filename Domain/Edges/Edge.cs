namespace Domain.Edges;

public class Edge<TNode>
{
    public required TNode BeginNode { get; set; }
    public required TNode EndNode { get; set; }
}