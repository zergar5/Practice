using Application.MathObjects.Equation;
using Domain.Environment;
using Domain.Nodes;
using System.Numerics;

namespace Application.FEM.Core.Assembling.Global.Sources;

public interface ISourceApplier<in TMatrix, T, TNode>
    where T : INumberBase<T>
    where TNode : Node
{
    public void Apply(IEquation<TMatrix, T> equation, Source<TNode> source);
}