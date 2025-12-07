using Application.InverseProblem.Parameters;
using Application.MathObjects.Vectors;
using Domain.Boundaries;
using Domain.Edges;
using Domain.Environment;
using Domain.Nodes;
using System.Numerics;

namespace Application.InverseProblem;

public interface IHarmonicInverseProblem<in TGridParameters, in TMaterial, TLocation> where TLocation : Node
{
    public void SetGridParameters(TGridParameters gridParameters);
    public void SetMaterials(TMaterial[] materials);
    public void SetDefinedValueBoundaryCondition(IDefinedValueBoundaryCondition<Edge<Node2D>, Complex>[] definedValueBoundaryCondition);
    public void SetSourcesAndReceivers(Source<TLocation>[] sources, ReceiverLine<TLocation>[] receiverLines);
    public void SetFrequencies(double[] frequencies);
    public void SetTargetParametersAndMeasurements(Parameter[] parameters, double[,] targetMeasurements);
    public IVector<double> Solve();
}