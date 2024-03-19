using System.Diagnostics;
using DirectProblem.Core;
using DirectProblem.Core.Base;
using DirectProblem.Core.Boundary;
using DirectProblem.Core.Global;
using DirectProblem.Core.GridComponents;
using DirectProblem.Core.Local;
using DirectProblem.FEM.Assembling;
using DirectProblem.FEM.Assembling.Local;
using DirectProblem.TwoDimensional.Assembling.Local;
using DirectProblem.TwoDimensional.Assembling.MatrixTemplates;
using System.Xml.Linq;

namespace DirectProblem.TwoDimensional.Assembling.Global;

public class GlobalAssembler<TNode>
{
    private readonly Grid<Node2D> _grid;
    private readonly IMatrixPortraitBuilder<TNode, SparseMatrix> _matrixPortraitBuilder;
    private readonly ILocalAssembler _localAssembler;
    private readonly IInserter<SparseMatrix> _inserter;
    private readonly GaussExcluder _gaussExсluder;
    private readonly LocalBasisFunctionsProvider _localBasisFunctionsProvider;
    private readonly Matrix _massTemplate;
    private readonly int[] _indexes = new int[2];
    private readonly Vector _bufferThetaVector = new(2);
    private readonly Vector _bufferVector = new(2);
    private readonly Vector _complexVector = new(4);
    private int[] _complexIndexes = new int[4];
    private Equation<SparseMatrix> _equation;
    private SparseMatrix _preconditionMatrix;

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
        _massTemplate = MassMatrixTemplateProvider.MassMatrix;
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

        //var threadsCount = ThreadPool.ThreadCount;

        //var tasks = new Task[threadsCount];

        //var thread = new Thread(() => { });

        //Action<object> ktok = (element) =>
        //{
        //    var localMatrix = _localAssembler.AssembleMatrix((Element)element);

        //    _inserter.InsertMatrix(_equation.Matrix, localMatrix);
        //};

        //for (var i = 0; i < threadsCount; i++)
        //{
        //    tasks[i] = new Task(() => {});
        //    tasks[i].Start();
        //}

        //foreach (var element in grid)
        //{
        //    var task = Task.WhenAny(tasks);
        //    task.ContinueWith((_, element) =>
        //    {
        //        var localMatrix = _localAssembler.AssembleMatrix((Element)element);

        //        _inserter.InsertMatrix(_equation.Matrix, localMatrix);
        //    }, element);
        //    //var task = new Task(ktok, element);
        //    //Task.WhenAny()
        //}

        foreach (var element in grid)
        {
            var localMatrix = _localAssembler.AssembleMatrix(element);

            _inserter.InsertMatrix(_equation.Matrix, localMatrix);
        }

        return this;
    }

    public GlobalAssembler<TNode> ApplySources(FocusedSource source)
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
        var stopwatch = new Stopwatch();
        stopwatch.Start();

        Parallel.ForEach(conditions, condition =>
        {
            _gaussExсluder.Exclude(_equation, condition);
        });

        stopwatch.Stop();

        var time = (double)stopwatch.ElapsedMilliseconds / 1000;

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
        var leftCornerNode = _grid.Nodes[element.NodesIndexes[0]];
        var rightCornerNode = _grid.Nodes[element.NodesIndexes[^1]];
        return node.R >= leftCornerNode.R && node.Z >= leftCornerNode.Z &&
               node.R <= rightCornerNode.R && node.Z <= rightCornerNode.Z;
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