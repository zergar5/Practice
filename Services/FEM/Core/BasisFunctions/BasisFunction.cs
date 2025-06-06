namespace Application.FEM.Core.BasisFunctions;

public interface IBasisFunction<in TNode>
{
    public double Evaluate(TNode node);
}