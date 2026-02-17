using Application.EquationSystems.Solvers.Sparse;
using System.Numerics;

namespace Application.DirectProblem._2D;

public interface IDirectProblem2DFactory<T, in TGridParameters, in TMaterial> where T : INumberBase<T>
{
    public IHarmonicDirectProblem2DWithSources<T, TGridParameters, TMaterial> Create(ISparseSLAESolver slaeSolver);
    public IHarmonicDirectProblem2DWithSources<T, TGridParameters, TMaterial>[] Create(Func<ISparseSLAESolver> slaeSolverFactoryMethod, int count);
}