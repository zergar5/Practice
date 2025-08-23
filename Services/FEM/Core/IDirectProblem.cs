using System.Numerics;

namespace Application.FEM.Core;

public interface IDirectProblem<out T, in TNode> where T : INumberBase<T>
{
    public IFEMSolutionResolver<T, TNode> Solve();
}