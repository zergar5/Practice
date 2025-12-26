using System.Numerics;

namespace Application.DirectProblem._2D;

public interface IDirectProblem2DFactory<T, in TGridParameters, in TMaterial> where T : INumberBase<T>
{
    public IHarmonicDirectProblem2DWithSources<T, TGridParameters, TMaterial> Create();
    public IHarmonicDirectProblem2DWithSources<T, TGridParameters, TMaterial>[] Create(int count);
}