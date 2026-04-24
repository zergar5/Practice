using System.Numerics;
using Application.DirectProblem;
using Domain.Environment;
using Domain.Nodes;
using System.Xml.Linq;
using Application.FEM._2D;
using Application.FEM.Core.Grid;

namespace Application.IO.Measurements;

public class HarmonicMeasurementsWriter2D
{
    private readonly string _path;

    public HarmonicMeasurementsWriter2D(string path)
    {
        _path = path;

        Directory.CreateDirectory(path);
    }

    public void WriteMeasurements(ReceiverLine<Node2D>[] receivers, double[,] measurements, double[] frequencies, double functional, string fileName)
    {
        using var streamWriterForPython =
            new StreamWriter(_path + fileName, new FileStreamOptions { Access = FileAccess.Write, Mode = FileMode.Create });

        foreach (var frequency in frequencies)
        {
            streamWriterForPython.Write($"{frequency / 1000000d:F6} ");
        }

        streamWriterForPython.WriteLine();

        foreach (var receiver in receivers)
        {
            streamWriterForPython.Write($"{receiver.ReceiverM.Z():F15} ");
        }

        streamWriterForPython.WriteLine();

        for (var i = 0; i < measurements.GetLength(0); i++)
        {
            for (var j = 0; j < measurements.GetLength(1); j++)
            {
                streamWriterForPython.Write($"{measurements[i, j]:E15} ");
            }

            streamWriterForPython.WriteLine();
        }

        streamWriterForPython.WriteLine($"{functional:E6}");
    }

    public void WriteMeasurements(double[] zPoints, double[] measurements, string fileName)
    {
        using var streamWriterForPython =
            new StreamWriter(_path + fileName, new FileStreamOptions { Access = FileAccess.Write, Mode = FileMode.Create });

        foreach (var zPoint in zPoints)
        {
            streamWriterForPython.Write($"{zPoint:F15} ");
        }

        streamWriterForPython.WriteLine();

        foreach (var measurement in measurements)
        {
            streamWriterForPython.Write($"{measurement:E15} ");
        }
    }

    public void WriteSinuses(ISolutionResolver<Complex, Node2D> solution, IGrid<Node2D, IElement2D> grid, string fileName)
    {
        using var binaryWriter = new BinaryWriter(File.Open(_path + fileName, FileMode.OpenOrCreate));

        foreach (var node in grid.Nodes)
        {
            binaryWriter.Write(solution.Get(node).Real);
        }
    }

    public void WriteCosinuses(ISolutionResolver<Complex, Node2D> solution, IGrid<Node2D, IElement2D> grid, string fileName)
    {
        using var binaryWriter = new BinaryWriter(File.Open(_path + fileName, FileMode.OpenOrCreate));

        foreach (var node in grid.Nodes)
        {
            binaryWriter.Write(solution.Get(node).Imaginary);
        }
    }
}