namespace Common.Extensions;

public static class DoubleExtensions
{
    public const double DefaultDoublePrecision = 1e-15;

    public static bool Equal(this double self, double other, double precision = DefaultDoublePrecision)
    {
        return Math.Abs(self - other) <= precision;
    }

    public static bool LessOrEqual(this double self, double other, double precision = DefaultDoublePrecision)
    {
        return self <= other || self.Equal(other, precision);
    }

    public static bool GreaterOrEqual(this double self, double other, double precision = DefaultDoublePrecision)
    {
        return self >= other || self.Equal(other, precision);
    }
}