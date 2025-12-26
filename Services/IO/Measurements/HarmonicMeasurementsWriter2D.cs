using Domain.Environment;
using Domain.Nodes;

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
}