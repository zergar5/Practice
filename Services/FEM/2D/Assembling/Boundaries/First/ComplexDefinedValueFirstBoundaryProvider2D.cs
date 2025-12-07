using Application.FEM._2D.Grid;
using Application.FEM.Assembling._2D.Boundaries.First;
using Domain.Boundaries;
using Domain.Edges;
using Domain.Enums;
using Domain.Nodes;
using System.Numerics;

namespace Application.FEM._2D.Assembling.Boundaries.First;

public class ComplexDefinedValueFirstBoundaryProvider2D : IDefinedValueFirstBoundaryProvider2D<Complex>
{
    public IDefinedValueBoundaryCondition<Edge<Node2D>, Complex>[] GetOnAllBounds(GridBuilder2D.Grid2DParameters gridParameters, Complex boundaryValue)
    {
        return [
            new EdgeBound<Node2D, Complex>
            {
                Attachment = new Edge<Node2D>
                {
                    BeginNode = new Node2D
                    {
                        X = gridParameters.XControlPoints[0],
                        Y = gridParameters.YControlPoints[0]
                    },
                    EndNode = new Node2D
                    {
                        X = gridParameters.XControlPoints[^1],
                        Y = gridParameters.YControlPoints[0]
                    }
                },
                Value = boundaryValue,
                Type = BoundaryConditionType.First
            },
            new EdgeBound<Node2D, Complex>
            {
                Attachment = new Edge<Node2D>
                {
                    BeginNode = new Node2D
                    {
                        X = gridParameters.XControlPoints[0],
                        Y = gridParameters.YControlPoints[0]
                    },
                    EndNode = new Node2D
                    {
                        X = gridParameters.XControlPoints[0],
                        Y = gridParameters.YControlPoints[^1]
                    }
                },
                Value = boundaryValue,
                Type = BoundaryConditionType.First
            },
            new EdgeBound<Node2D, Complex>
            {
                Attachment = new Edge<Node2D>
                {
                    BeginNode = new Node2D
                    {
                        X = gridParameters.XControlPoints[^1],
                        Y = gridParameters.YControlPoints[0]
                    },
                    EndNode = new Node2D
                    {
                        X = gridParameters.XControlPoints[^1],
                        Y = gridParameters.YControlPoints[^1]
                    }
                },
                Value = boundaryValue,
                Type = BoundaryConditionType.First
            },
            new EdgeBound<Node2D, Complex>
            {
                Attachment = new Edge<Node2D>
                {
                    BeginNode = new Node2D
                    {
                        X = gridParameters.XControlPoints[0],
                        Y = gridParameters.YControlPoints[^1]
                    },
                    EndNode = new Node2D
                    {
                        X = gridParameters.XControlPoints[^1],
                        Y = gridParameters.YControlPoints[^1]
                    }
                },
                Value = boundaryValue,
                Type = BoundaryConditionType.First
            }
        ];
    }
}