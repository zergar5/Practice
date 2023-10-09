using DirectProblem.Core;
using DirectProblem.Core.Base;
using DirectProblem.Core.Boundary;
using DirectProblem.Core.Global;
using DirectProblem.Core.GridComponents;
using DirectProblem.Core.Local;
using DirectProblem.FEM.Assembling;
using DirectProblem.FEM.Assembling.Local;
using DirectProblem.TwoDimensional.Assembling.Local;

namespace DirectProblem.TwoDimensional.Assembling.Global;

public class GlobalAssembler<TNode>
{
    private readonly Grid<Node2D> _grid;
    private readonly IMatrixPortraitBuilder<TNode, SparseMatrix> _matrixPortraitBuilder;
    private readonly ILocalAssembler _localAssembler;
    private readonly IInserter<SparseMatrix> _inserter;
    private readonly GaussExcluder _gaussExсluder;
    private readonly LocalBasisFunctionsProvider _localBasisFunctionsProvider;
    private Equation<SparseMatrix> _equation;
    private SparseMatrix _preconditionMatrix;
    private Vector _bufferVector = new(4);

    public GlobalAssembler
    (
        Grid<Node2D> grid,
        IMatrixPortraitBuilder<TNode, SparseMatrix> matrixPortraitBuilder,
        ILocalAssembler localAssembler,
        IInserter<SparseMatrix> inserter,
        GaussExcluder gaussExсluder,
        LocalBasisFunctionsProvider localBasisFunctionsProvider
    )
    {
        _grid = grid;
        _matrixPortraitBuilder = matrixPortraitBuilder;
        _localAssembler = localAssembler;
        _inserter = inserter;
        _gaussExсluder = gaussExсluder;
        _localBasisFunctionsProvider = localBasisFunctionsProvider;
    }

    public GlobalAssembler<TNode> AssembleEquation(Grid<TNode> grid)
    {
        var globalMatrix = _matrixPortraitBuilder.Build(grid);
        _preconditionMatrix = globalMatrix.Clone();
        _equation = new Equation<SparseMatrix>(
            globalMatrix,
            new Vector(grid.Nodes.Length * 2),
            new Vector(grid.Nodes.Length * 2)
        );

        foreach (var element in grid)
        {
            var localMatrix = _localAssembler.AssembleMatrix(element);

            _inserter.InsertMatrix(_equation.Matrix, localMatrix);
        }
        return this;
    }

    public GlobalAssembler<TNode> ApplySources(FocusedSource[] sources)
    {
        foreach (var source in sources)
        {
            var element = _grid.Elements.First(x => ElementHas(x, source.Point));

            var basisFunctions = _localBasisFunctionsProvider.GetBilinearFunctions(element);

            for (var i = 0; i < element.NodesIndexes.Length; i++)
            {
                _bufferVector[i] = source.Current * basisFunctions[i].Calculate(source.Point);
            }

            _inserter.InsertVector(_equation.RightPart, new LocalVector(element.NodesIndexes, _bufferVector));
        }


        return this;
    }

    public GlobalAssembler<TNode> ApplyFirstConditions(List<FirstConditionValue> conditions)
    {
        foreach (var condition in conditions)
        {
            _gaussExсluder.Exclude(_equation, condition);
        }

        return this;
    }

    public Equation<SparseMatrix> BuildEquation()
    {
        return _equation;
    }

    public SparseMatrix BuildPreconditionMatrix()
    {
        _preconditionMatrix = _equation.Matrix.Copy(_preconditionMatrix);
        return _preconditionMatrix;
    }

    private bool ElementHas(Element element, Node2D node)
    {
        var leftCornerNode = _grid.Nodes[element.NodesIndexes[0]];
        var rightCornerNode = _grid.Nodes[element.NodesIndexes[^1]];
        return node.R >= leftCornerNode.R && node.Z >= leftCornerNode.Z &&
               node.R <= rightCornerNode.R && node.Z <= rightCornerNode.Z;
    }
}