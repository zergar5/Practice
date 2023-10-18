using DirectProblem.Core.GridComponents;
using System.Drawing;

namespace DirectProblem.IO;

public class ResultIO
{
    private readonly string _path;

    public ResultIO(string path)
    {
        _path = path;
    }

    public void Write(string fileName, double omega, Node2D[] points, (double, double)[] emfs)
    {
        using var streamWriter = new StreamWriter(_path + fileName);

        streamWriter.WriteLine(omega / 1000000d);

        foreach (var point in points)
        {
            streamWriter.Write($"{point.Z} ");
        }
        streamWriter.WriteLine();

        foreach (var emfSin in emfs)
        {
            streamWriter.Write($"{emfSin.Item1} ");
        }
        streamWriter.WriteLine();

        foreach (var emfCos in emfs)
        {
            streamWriter.Write($"{emfCos.Item1} ");
        }
    }
}