using DirectProblem.Core.GridComponents;

namespace DirectProblem.FEM;

public class CourseHolder
{
    public static void GetResidualInfo(int iteration, double residual)
    {
        Console.Write($"Iteration: {iteration}, residual: {residual:E14}                                   \r");
    }

    public static void GetFunctionalInfo(int iteration, double functional)
    {
        Console.Write($"Iteration: {iteration}, functional: {functional:E14}                                   \r");
    }


    public static void WriteSolution(Node2D point, (double sValue, double cValue) values)
    {
        Console.WriteLine($"({point.R:F2},{point.Z:F2}) EMFs = {values.sValue:E14} EMFc = {values.cValue:E14}");
    }

    public static void WriteAreaInfo()
    {
        Console.WriteLine("Point not in area or time interval");
    }
}