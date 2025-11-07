namespace Common.Extensions;

public static class DoubleExtensions
{
    public static bool EqualsWithPrecision(this double self, double other, double precision = 1e-15)
    {
        return Math.Abs(self - other) <= precision;
    }
}