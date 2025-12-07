using Application.FEM._2D;
using Domain.Edges;
using Domain.Environment;
using Domain.Nodes;
using System.Numerics;

namespace Application.DirectProblem._2D;

public interface IDirectProblem2D<T, in TGridParameters, in TMaterial> : IDirectProblem<T, Node2D, IElement2D, TGridParameters, Edge<Node2D>, TMaterial>
    where T : INumberBase<T>;

public interface IDirectProblem2DWithSources<T, in TGridParameters, in TMaterial> : IDirectProblem2D<T, TGridParameters, TMaterial>
    where T : INumberBase<T>
{
    public void SetSources(Source<Node2D>[] sources);
}

public interface IHarmonicDirectProblem2D<T, in TGridParameters, in TMaterial> : IDirectProblem2D<T, TGridParameters, TMaterial>
    where T : INumberBase<T>
{
    public void SetFrequency(double frequency);
}

public interface IHarmonicDirectProblem2DWithSources<T, in TGridParameters, in TMaterial> :
    IDirectProblem2DWithSources<T, TGridParameters, TMaterial>,
    IHarmonicDirectProblem2D<T, TGridParameters, TMaterial>
    where T : INumberBase<T>;