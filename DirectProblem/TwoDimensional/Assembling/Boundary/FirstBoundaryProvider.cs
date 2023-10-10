using DirectProblem.Core;
using DirectProblem.Core.Boundary;
using DirectProblem.Core.GridComponents;
using System;
using DirectProblem.Core.Base;
using DirectProblem.Core.Local;

namespace DirectProblem.TwoDimensional.Assembling.Boundary;

public class FirstBoundaryProvider
{
    private readonly Grid<Node2D> _grid;
    private int[][]? _indexes;
    private Vector[]? _values;
    private readonly int[] _indexesBuffer = new int [2];

    public FirstBoundaryProvider(Grid<Node2D> grid)
    {
        _grid = grid;
    }

    public FirstConditionValue[] GetConditions(FirstCondition[] conditions)
    {
        var conditionsValues = new FirstConditionValue[conditions.Length * 2];

        if (_indexes is null)
        {
            _indexes = new int[conditionsValues.Length][];

            for (var i = 0; i < conditionsValues.Length; i++)
            {
                _indexes[i] = new int[2];
            }
        }

        if (_values is null)
        {
            _values = new Vector[conditionsValues.Length];

            for (var i = 0; i < conditionsValues.Length; i++)
            {
                _values[i] = new Vector(2);
            }
        }

        var j = 0;
        foreach (var condition in conditions)
        {
            var (indexes, _) = _grid.Elements[condition.ElementIndex].GetBoundNodeIndexes(condition.Bound, _indexesBuffer);

            for (var i = 0; i < 2; i++, j++)
            {
                for (var k = 0; k < indexes.Length; k++)
                {
                    _indexes[j][k] = indexes[k] * 2 + i;
                }

                conditionsValues[j] = new FirstConditionValue(new LocalVector(_indexes[j], _values[j]));
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