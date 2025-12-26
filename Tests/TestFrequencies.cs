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

    public static double[] GetSixFrequencies()
    {
        return [4e4, 2e5, 6e5, 1e6, 1.5e6, 2e6];
    }

    public static double[] GetSevenFrequencies()
    {
        return [4e4, 8e4, 1.6e5, 3.2e5, 6.4e5, 1.28e6, 2e6];
    }

    public static double[] GetEightFrequencies()
    {
        return [4e4, 3e5, 6e5, 9e5, 1.2e6, 1.5e6, 1.8e6, 2e6];
    }

    public static double[] GetElevenFrequencies()
    {
        return [4e4, 2e5, 4e5, 6e5, 8e5, 1e6, 1.2e6, 1.4e6, 1.6e6, 1.8e6, 2e6];
    }

    public static double[] GetFrequencies()
    {
        return [4e4, 8e4, 1.6e5, 3.2e5, 6.4e5, 1.28e6, 2e6];
    }
}