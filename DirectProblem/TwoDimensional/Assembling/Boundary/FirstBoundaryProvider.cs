using DirectProblem.Core;
using DirectProblem.Core.Boundary;
using DirectProblem.Core.GridComponents;
using System;
using DirectProblem.Core.Base;

namespace DirectProblem.TwoDimensional.Assembling.Boundary;

public class FirstBoundaryProvider
{
    private readonly Grid<Node2D> _grid;
    private Vector[]? _buffers;

    public FirstBoundaryProvider(Grid<Node2D> grid)
    {
        _grid = grid;
    }

    public FirstConditionValue[] GetConditions(FirstCondition[] conditions)
    {
        var conditionsValues = new FirstConditionValue[conditions.Length * 2];

        if (_buffers is null)
        {
            _buffers = new Vector[conditionsValues.Length * 2];

            for (var i = 0; i < conditions.Length * 2; i++)
            {
                _buffers[i] = new Vector(2);
            }
        }

        for (var j = 0; j < conditions.Length * 2;)
        {
            var (indexes, _) = _grid.Elements[conditions[j].ElementIndex].GetBoundNodeIndexes(conditions[j].Bound);

            foreach (var i in indexes)
            {
                conditions[j++] = new FirstConditionValue(j * 2, 0);
            }
        }

        return conditionsValues;
    }

    public FirstConditionValue[] GetConditions(int elementsByLength, int elementsByHeight)
    {
        var conditions = new FirstCondition[2 * (elementsByLength + elementsByHeight)];

        var j = 0;
        for (var i = 0; i < elementsByLength; i++, j++)
        {
            conditions[j] = new FirstCondition(i, Bound.Lower);
        }

        for (var i = 0; i < elementsByHeight; i++, j++)
        {
            conditions[j] = new FirstCondition(i * elementsByLength, Bound.Left);
        }

        for (var i = 0; i < elementsByHeight; i++, j++)
        {
            conditions[j] = new FirstCondition((i + 1) * elementsByLength - 1, Bound.Right);
        }

        for (var i = elementsByLength * (elementsByHeight - 1); i < elementsByLength * elementsByHeight; i++, j++)
        {
            conditions[j] = new FirstCondition(i, Bound.Upper);
        }

        return GetConditions(conditions);
    }
}