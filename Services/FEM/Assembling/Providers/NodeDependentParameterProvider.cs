using System.Numerics;

namespace Application.FEM.Assembling.Providers;

public interface INodeDependentParameterProvider<out TResult> where TResult : INumber<TResult>
{
    public TResult Get(int nodeIndex);
}

public class NodeDependentParameterProvider<TResult, TNode> : INodeDependentParameterProvider<TResult>
    where TResult : INumber<TResult>
{
    private readonly TNode[] _nodes;
    private readonly Func<TNode, TResult> _function;

    public NodeDependentParameterProvider(TNode[] nodes, Func<TNode, TResult> function)
    {
        _nodes = nodes;
        _function = function;
    }

    public TResult Get(int nodeIndex) => _function(_nodes[nodeIndex]);
}