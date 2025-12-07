using Common.Extensions;

namespace Domain.Nodes;

public class Node2D : Node
{
    public double Y { get; set; }
}

public static class Node2DExtensions
{
    public static double R(this Node2D node) => node.X;
    public static double Z(this Node2D node) => node.Y;

    public static bool EqualsWithPrecision(this Node2D self, Node2D other, double precision = 1e-15)
    {
        return self.X.Equal(other.X, precision) && self.Y.Equal(other.Y, precision);
    }
}