using System.Numerics;

namespace Application.FEM.Core;

public interface IFEMSolutionResolver<out T, in TNode> where T : INumberBase<T>
{
    public T Get(TNode node);
}