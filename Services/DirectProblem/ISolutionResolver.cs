using System.Numerics;

namespace Application.DirectProblem;

public interface ISolutionResolver<out T, in TNode> where T : INumberBase<T>
{
    public T Get(TNode node);
}