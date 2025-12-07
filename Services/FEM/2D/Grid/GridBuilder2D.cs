using Application.FEM.Core.Grid;
using Application.FEM.Core.Grid.Splitting;
using Common.Extensions;
using Domain.Areas;
using Domain.Nodes;
using static Application.FEM._2D.Grid.GridBuilder2D;

namespace Application.FEM._2D.Grid;

public class GridBuilder2D : IGridBuilder<Node2D, IElement2D, Grid2DParameters>
{
    private readonly AxisSplitter _axisSplitter;

    public GridBuilder2D(AxisSplitter axisSplitter)
    {
        _axisSplitter = axisSplitter;
    }

    public IGrid<Node2D, IElement2D> Build(Grid2DParameters gridParameters)
    {
        var xAxis = _axisSplitter.SplitAxis(gridParameters.XControlPoints, gridParameters.XSplitStrategies).ToArray();
        var yAxis = _axisSplitter.SplitAxis(gridParameters.YControlPoints, gridParameters.YSplitStrategies).ToArray();
        var nodes = GenerateNodes(xAxis, yAxis).ToArray();
        var elements = GenerateElements(gridParameters, nodes, xAxis.Length, yAxis.Length);

        return new Grid<Node2D, IElement2D>(nodes, elements);
    }

    private static IEnumerable<Node2D> GenerateNodes(double[] xAxis, double[] yAxis)
    {
        foreach (var yNode in yAxis)
        {
            foreach (var xNode in xAxis)
            {
                yield return new Node2D { X = xNode, Y = yNode };
            }
        }
    }

    private IElement2D[] GenerateElements(Grid2DParameters gridParameters, Node2D[] nodes, int totalXNodes, int totalYNodes)
    {
        var totalXElements = totalXNodes - 1;
        var totalYElements = totalYNodes - 1;
        var totalElements = totalXElements * totalYElements;

        var elements = new IElement2D[totalElements];

        for (var topRow = 1; topRow < totalYNodes; topRow++)
        {
            var bottomRow = topRow - 1;

            for (var rightColumn = 1; rightColumn < totalXNodes; rightColumn++)
            {
                var leftColumn = rightColumn - 1;
                var elementIndex = leftColumn + bottomRow * totalXElements;
                var nodesIndexes = GetNodesIndexes(leftColumn, rightColumn, bottomRow, topRow, totalXNodes);

                var leftBottom = nodes[nodesIndexes[0]];
                var leftTop = nodes[nodesIndexes[2]];
                var rightBottom = nodes[nodesIndexes[1]];
                var rightTop = nodes[nodesIndexes[3]];

                var length = rightBottom.X - leftBottom.X;
                var height = leftTop.Y - leftBottom.Y;

                var materialId = GetElementMaterial(leftBottom, rightTop, gridParameters);

                elements[elementIndex] = new Element2D(materialId, nodesIndexes, length, height);
            }
        }

        return elements;
    }

    private static int[] GetNodesIndexes(int leftColumn, int rightColumn, int bottomRow, int topRow, int totalXNodes)
    {
        var indexes = new int[4];

        indexes[0] = leftColumn + bottomRow * totalXNodes;
        indexes[1] = rightColumn + bottomRow * totalXNodes;
        indexes[2] = leftColumn + topRow * totalXNodes;
        indexes[3] = rightColumn + topRow * totalXNodes;

        return indexes;
    }

    private static int GetElementMaterial(Node2D leftBottom, Node2D rightTop, Grid2DParameters gridParameters)
    {
        var xControlPoints = gridParameters.XControlPoints;
        var yControlPoints = gridParameters.YControlPoints;
        var areas = gridParameters.Areas;

        var area = areas.First(a =>
            xControlPoints[a.XStartControlPointId].LessOrEqual(leftBottom.X) &&
            yControlPoints[a.YStartControlPointId].LessOrEqual(leftBottom.Y) &&
            rightTop.X.LessOrEqual(xControlPoints[a.XEndControlPointId]) &&
            rightTop.Y.LessOrEqual(yControlPoints[a.YEndControlPointId])
        );

        return area.MaterialId;
    }

    public class Grid2DParameters
    {
        public required double[] XControlPoints { get; set; }
        public required double[] YControlPoints { get; set; }
        public required ISplitStrategy[] XSplitStrategies { get; set; }
        public required ISplitStrategy[] YSplitStrategies { get; set; }
        public required Area2D[] Areas { get; set; }
    }
}