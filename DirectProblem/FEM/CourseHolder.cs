using DirectProblem.Core.GridComponents;

namespace DirectProblem.FEM;

public class CourseHolder
{
    public static void GetInfo(int iteration, double residual)
    {
        Console.Write($"Iteration: {iteration}, residual: {residual:E14}                                   \r");
    }

    public static void WriteSolution(Node2D point, (double sValue, double cValue) values)
    {
        Console.WriteLine($"({point.R:F2},{point.Z:F2}) As = {values.sValue:E14} Ac = {values.cValue:E14}");
    }

    public static void WriteSolution(Node2D point, double time, double value)
    {
        Console.WriteLine($"({point.R:F2},{point.Z:F2}) {time} {value:E14}");
    }

    public static void WriteAreaInfo()
    {
        Console.WriteLine("Point not in area or time interval");
    }
}