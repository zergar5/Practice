using DirectProblem.Core;
using DirectProblem.Core.Base;
using DirectProblem.Core.Boundary;
using DirectProblem.Core.Global;
using DirectProblem.Core.GridComponents;
using DirectProblem.Core.Local;
using DirectProblem.FEM.Assembling;
using DirectProblem.FEM.Assembling.Local;
using DirectProblem.SLAE;
using DirectProblem.TwoDimensional.Assembling.MatrixTemplates;

namespace DirectProblem.TwoDimensional.Assembling.Global;

public class GlobalAssembler<TNode>
{
    private readonly Grid<Node2D> _grid;
    private readonly IMatrixPortraitBuilder<TNode, SparseMatrix> _matrixPortraitBuilder;
    private readonly ILocalAssembler _localAssembler;
    private readonly IInserter<SparseMatrix> _inserter;
    private readonly GaussExcluder _gaussExсluder;

    private readonly Matrix _massTemplate;
    private readonly int[] _indexes;
    private readonly Vector _bufferThetaVector;
    private readonly Vector _bufferVector;
    private readonly Vector _complexVector;
    private int[] _complexIndexes;

    private Equation<SparseMatrix> _equation;
    private SparseMatrix _preconditionMatrix;

    public GlobalAssembler
    (
        Grid<Node2D> grid,
        IMatrixPortraitBuilder<TNode, SparseMatrix> matrixPortraitBuilder,
        ILocalAssembler localAssembler,
        IInserter<SparseMatrix> inserter,
        GaussExcluder gaussExсluder
    )
    {
        _grid = grid;
        _matrixPortraitBuilder = matrixPortraitBuilder;
        _localAssembler = localAssembler;
        _inserter = inserter;
        _gaussExсluder = gaussExсluder;
        _massTemplate = MassMatrixTemplateProvider.MassMatrix;
        _indexes = new int[2];
        _bufferThetaVector = new Vector(2);
        _bufferVector = new Vector(2);
        _complexVector = new Vector(4);
        _complexIndexes = new int[4];
    }

    public GlobalAssembler<TNode> AssembleEquation(Grid<TNode> grid)
    {
        var globalMatrix = _matrixPortraitBuilder.Build(grid);

        if (_preconditionMatrix is null || _preconditionMatrix.Count != globalMatrix.Count)
        {
            _preconditionMatrix = globalMatrix.Clone();
        }

        if (_equation is null || _equation.RightPart.Count != grid.Nodes.Length * 2)
        {
            _equation = new Equation<SparseMatrix>(
                globalMatrix,
                new Vector(grid.Nodes.Length * 2),
                new Vector(grid.Nodes.Length * 2)
            );
        }

        if (_equation.Matrix.Equals(globalMatrix))
        {
            Console.WriteLine("Lel");
        }

        globalMatrix.Copy(_equation.Matrix);
        _equation.RightPart.Clear();

        foreach (var element in grid)
        {
            var localMatrix = _localAssembler.AssembleMatrix(element);

            _inserter.InsertMatrix(_equation.Matrix, localMatrix);
        }

        return this;
    }

    public GlobalAssembler<TNode> ApplySources(Source source)
    {
        var element = _grid.Elements.First(x => ElementHas(x, source.Point));

        var theta = source.Current / (2 * Math.PI * source.Point.R * element.Height);

        element.GetBoundNodeIndexes(Bound.Right, _indexes);

        for (var i = 0; i < _bufferThetaVector.Count; i++)
        {
            _bufferThetaVector[i] = theta;
        }

        var mass = Matrix.Multiply(element.Height * source.Point.R / 6d,
            _massTemplate);

        Matrix.Multiply(mass, _bufferThetaVector, _bufferVector);

        _complexIndexes = GetComplexIndexes(_indexes);

        for (var i = 0; i < _indexes.Length; i++)
        {
            _complexVector[i * 2] = _bufferVector[i];
            _complexVector[i * 2 + 1] = 0d;
        }

        _inserter.InsertVector(_equation.RightPart, new LocalVector(_complexIndexes, _complexVector));

        return this;
    }

    public GlobalAssembler<TNode> ApplyFirstConditions(FirstConditionValue[] conditions)
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

    public SparseMatrix AllocatePreconditionMatrix()
    {
        _preconditionMatrix = _equation.Matrix.Copy(_preconditionMatrix);
        return _preconditionMatrix;
    }

    private bool ElementHas(Element element, Node2D node)
    {
        var lowerLeftCorner = _grid.Nodes[element.NodesIndexes[0]];
        var upperRightCorner = _grid.Nodes[element.NodesIndexes[^1]];
        return (node.R > lowerLeftCorner.R ||
                Math.Abs(node.R - lowerLeftCorner.R) < MethodsConfig.EpsDouble) &&
               (node.Z > lowerLeftCorner.Z ||
                Math.Abs(node.Z - lowerLeftCorner.Z) < MethodsConfig.EpsDouble) &&
               (node.R < upperRightCorner.R ||
                Math.Abs(node.R - upperRightCorner.R) < MethodsConfig.EpsDouble) &&
               (node.Z < upperRightCorner.Z ||
                Math.Abs(node.Z - upperRightCorner.Z) < MethodsConfig.EpsDouble);
    }

    private int[] GetComplexIndexes(int[] indexes)
    {
        for (var i = 0; i < indexes.Length; i++)
        {
            _complexIndexes[i * 2] = 2 * indexes[i];
            _complexIndexes[i * 2 + 1] = _complexIndexes[i * 2] + 1;
        }

        return _complexIndexes;
    }
}