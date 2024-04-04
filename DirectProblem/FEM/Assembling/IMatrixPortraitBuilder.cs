using DirectProblem.Core;

namespace DirectProblem.FEM.Assembling;

public interface IMatrixPortraitBuilder<TNode, out TMatrix>
{
    TMatrix Build(Grid<TNode> grid);
}