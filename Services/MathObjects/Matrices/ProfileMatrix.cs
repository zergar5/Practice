using System.Numerics;

namespace Application.MathObjects.Matrices;

public interface IProfileMatrix<T> where T : INumberBase<T>
{
    public int RowCount { get; }
    public int ColumnCount { get; }
    public IReadOnlyList<int> RowIndexes { get; }
    public T[] Diagonal { get; }
    public List<T> LowerValues { get; }
    public List<T> UpperValues { get; }
}

public class ProfileMatrix<T> : IProfileMatrix<T> where T : INumberBase<T>
{
    protected readonly int[] RowIndexesInner;

    public IReadOnlyList<int> RowIndexes => RowIndexesInner.AsReadOnly();
    public int RowCount => Diagonal.Length;
    public int ColumnCount => Diagonal.Length;
    public T[] Diagonal { get; }
    public List<T> LowerValues { get; }
    public List<T> UpperValues { get; }

    public ProfileMatrix(int[] rowsIndexes, T[] diagonal, List<T> lowerValues, List<T> upperValues)
    {
        RowIndexesInner = rowsIndexes;
        Diagonal = diagonal;
        LowerValues = lowerValues;
        UpperValues = upperValues;
    }
}