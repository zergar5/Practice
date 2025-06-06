using Application.FEM._1D;
using Application.FEM.Core.Grid;
using Domain.Enums;

namespace Application.FEM._2D;

public interface IElement2D : IElement
{
    public double Height { get; }
}

public class Element2D : Element, IElement2D
{
    public double Height { get; }

    public Element2D(int materialId, int[] nodeIndexes, double length, double height) : base(materialId, nodeIndexes, length)
    {
        Height = height;
    }

    public BoundInfo GetBoundNodeIndexes(Bound2D bound, int[]? indexes = null)
    {
        indexes ??= new int[2];
        var boundSize = Length;

        switch (bound)
        {
            case Bound2D.Lower:
                indexes[0] = NodeIndexes[0];
                indexes[1] = NodeIndexes[1];
                break;
            case Bound2D.Left:
                indexes[0] = NodeIndexes[0];
                indexes[1] = NodeIndexes[2];
                boundSize = Height;
                break;
            case Bound2D.Right:
                indexes[0] = NodeIndexes[1];
                indexes[1] = NodeIndexes[3];
                boundSize = Height;
                break;
            case Bound2D.Upper:
                indexes[0] = NodeIndexes[2];
                indexes[1] = NodeIndexes[3];
                break;
        }

        return new BoundInfo
        {
            Bound = bound,
            Size = boundSize,
        };
    }

    public class BoundInfo
    {
        public Bound2D Bound { get; set; }
        public double Size { get; set; }
    }
}