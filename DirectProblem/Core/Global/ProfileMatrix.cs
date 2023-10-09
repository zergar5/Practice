﻿namespace Practice6Sem.Core.Global;

public class ProfileMatrix
{
    public double[] Diagonal { get; }
    public List<double> LowerValues { get; }
    public List<double> UpperValues { get; }
    public int[] RowsIndexes { get; }

    public int Count => Diagonal.Length;

    //public ReadOnlySpan<int> this[int rowIndex] => newRowsIndexes[rowIndex]..RowsIndexes[rowIndex + 1]];

    public ProfileMatrix(int[] rowsIndexes, double[] diagonal, List<double> lowerValues, List<double> upperValues)
    {
        RowsIndexes = rowsIndexes;
        Diagonal = diagonal;
        LowerValues = lowerValues;
        UpperValues = upperValues;
    }

    public ProfileMatrix LU()
    {
        for (var i = 0; i < Count; i++)
        {
            var j = i - (RowsIndexes[i + 1] - RowsIndexes[i]);

            var sumD = 0d;

            for (var ij = RowsIndexes[i]; ij < RowsIndexes[i + 1]; ij++, j++)
            {
                var sumL = 0d;
                var sumU = 0d;

                var k = j - (RowsIndexes[j + 1] - RowsIndexes[j]);

                var ik = RowsIndexes[i];
                var kj = RowsIndexes[j];

                if (k - (i - (RowsIndexes[i + 1] - RowsIndexes[i])) < 0) kj -= k - (i - (RowsIndexes[i + 1] - RowsIndexes[i]));
                else ik += k - (i - (RowsIndexes[i + 1] - RowsIndexes[i]));

                for (; ik < ij; ik++, kj++)
                {
                    sumL += LowerValues[ik] * UpperValues[kj];
                    sumU += LowerValues[kj] * UpperValues[ik];
                }

                LowerValues[ij] -= sumL;
                UpperValues[ij] = (UpperValues[ij] - sumU) / Diagonal[j];

                sumD += LowerValues[ij] * UpperValues[ij];
            }

            Diagonal[i] -= sumD;

        }

        return this;
    }
}