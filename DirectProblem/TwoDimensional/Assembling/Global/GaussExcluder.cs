﻿using DirectProblem.Core.Boundary;
using DirectProblem.Core.Global;
using DirectProblem.FEM.Assembling.Global;

namespace DirectProblem.TwoDimensional.Assembling.Global;

public class GaussExcluder : IGaussExcluder<SparseMatrix>
{
    public void Exclude(Equation<SparseMatrix> equation, FirstConditionValue condition)
    {
        for (var i = 0; i < condition.Values.Count; i++)
        {
            var row = condition.Values.Indexes[i];
            equation.RightPart[row] = condition.Values[i];
            equation.Matrix[row, row] = 1d;

            foreach (var j in equation.Matrix[row])
            {
                equation.Matrix[row, j] = 0d;
            }

            for (var column = row + 1; column < equation.Matrix.Count; column++)
            {
                if (!equation.Matrix[column].Contains(row)) continue;

                equation.Matrix[row, column] = 0d;
            }
        }
    }
}