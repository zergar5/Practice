using Domain.Materials;

namespace Tests;

public class TestFrequencies
{
    public static double[] GetOneFrequency()
    {
        return [4e4];
    }

    public static double[] GetTwoFrequencies()
    {
        return [4e4, 2e5];
    }

    public static double[] GetFourFrequencies()
    {
        return [4e4, 2e5, 1e6, 2e6];
    }
}