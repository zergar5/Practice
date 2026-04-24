using System.Numerics;
using Application.MathObjects.Vectors;

namespace Application.DirectProblem;

public interface ISolutionResolver<out T, in TNode> where T : INumberBase<T>
{
    public T Get(TNode node);
}