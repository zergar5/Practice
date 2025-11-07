namespace Application.FEM.Core.Assembling.Boundaries;

public interface IBoundCoverageResolver<in TAttachment>
{
    public int[] ResolveNodeIds(TAttachment attachment);
}