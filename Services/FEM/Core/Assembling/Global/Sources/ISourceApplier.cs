using Application.MathObjects.Equation;
using Domain.Environment;
using Domain.Nodes;

namespace Application.FEM.Core.Assembling.Global.Sources;

public interface ISourceApplier<in TMatrix, TNode> where TNode : Node
{
    public void Apply(IEquation<TMatrix, double> equation, Source<TNode> source);
}