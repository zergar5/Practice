namespace Common.Extensions;

public static class DoubleExtensions
{
    public const double DefaultDoublePrecision = 1e-15;

    public static bool EqualTo(this double self, double other, double precision = DefaultDoublePrecision)
    {
        return Math.Abs(self - other) <= precision;
    }

    public static bool LessOrEqualThan(this double self, double other, double precision = DefaultDoublePrecision)
    {
        return self <= other || self.EqualTo(other, precision);
    }

    public static bool GreaterOrEqualThan(this double self, double other, double precision = DefaultDoublePrecision)
    {
        return self >= other || self.EqualTo(other, precision);
    }

    public static double Difference(this double self, double other)
    {
        return Math.Sqrt(Math.Pow(self - other, 2));
    }
}