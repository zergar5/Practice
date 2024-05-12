using DirectProblem.Core.GridComponents;
using System.Numerics;
using Vector = DirectProblem.Core.Base.Vector;

namespace DirectProblem.IO;

public class ResultIO
{
    private readonly string _path;
    private readonly string[] _fileNames = { "40kHz", "200kHz", "1MHz", "2Mhz" };

    public ResultIO(string path)
    {
        _path = path;
    }

    public void Write(string fileName, double[] omegas, double[] centersZ, Complex[,] emfs)
    {
        using var streamWriterForPython = new StreamWriter(_path + fileName);

        foreach (var omega in omegas)
        {
            streamWriterForPython.Write($"{omega / 1000000d} ");
        }

        streamWriterForPython.WriteLine();

        foreach (var centerZ in centersZ)
        {
            streamWriterForPython.Write($"{centerZ} ");
        }

        streamWriterForPython.WriteLine();

        for (var i = 0; i < emfs.GetLength(1); i++)
        {
            for (var j = 0; j < emfs.GetLength(0); j++)
            {
                streamWriterForPython.Write($"{emfs[j, i].Real} ");
            }

            streamWriterForPython.WriteLine();
        }

        for (var i = 0; i < emfs.GetLength(1); i++)
        {
            for (var j = 0; j < emfs.GetLength(0); j++)
            {
                streamWriterForPython.Write($"{emfs[j, i].Imaginary} ");
            }

            streamWriterForPython.WriteLine();
        }

        for (var i = 0; i < omegas.Length; i++)
        {
            using var streamWriterForTelmaSin = new StreamWriter(_path + "sin" + _fileNames[i] + ".txt");

            streamWriterForTelmaSin.WriteLine();
            streamWriterForTelmaSin.WriteLine();

            for (var j = 0; j < centersZ.Length; j++)
            {
                streamWriterForTelmaSin.WriteLine($"{centersZ[j]} {emfs[j, i].Real}");
            }

            using var streamWriterForTelmaCos = new StreamWriter(_path + "cos" + _fileNames[i] + ".txt");

            streamWriterForTelmaCos.WriteLine();
            streamWriterForTelmaCos.WriteLine();

            for (var j = 0; j < centersZ.Length; j++)
            {
                streamWriterForTelmaCos.WriteLine($"{centersZ[j]} {emfs[j, i].Imaginary}");
            }
        }
    }

    public void Write(string fileName, double[] omegas, double[] centersZ, double[,] phaseDifferences)
    {
        using var streamWriterForPython = new StreamWriter(_path + fileName);

        foreach (var omega in omegas)
        {
            streamWriterForPython.Write($"{omega / 1000000d} ");
        }

        streamWriterForPython.WriteLine();

        foreach (var centerZ in centersZ)
        {
            streamWriterForPython.Write($"{centerZ} ");
        }

        streamWriterForPython.WriteLine();

        for (var i = 0; i < phaseDifferences.GetLength(1); i++)
        {
            for (var j = 0; j < phaseDifferences.GetLength(0); j++)
            {
                streamWriterForPython.Write($"{phaseDifferences[j, i]} ");
            }

            streamWriterForPython.WriteLine();
        }

        for (var i = 0; i < omegas.Length; i++)
        {
            using var streamWriterForTelma =
                new StreamWriter(_path + "phaseDifferences" + _fileNames[i] + ".txt");

            streamWriterForTelma.WriteLine();
            streamWriterForTelma.WriteLine();

            for (var j = 0; j < centersZ.Length; j++)
            {
                streamWriterForTelma.WriteLine($"{centersZ[j]} {phaseDifferences[j, i]}");
            }
        }

    }

    public void WriteSinuses(Vector solution, string fileName)
    {
        using var binaryWriter = new BinaryWriter(File.Open(_path + fileName, FileMode.OpenOrCreate));

        for (var i = 0; i < solution.Count; i++)
        {
            if (i % 2 == 0) binaryWriter.Write(solution[i]);
        }
    }

    public void WriteCosinuses(Vector solution, string fileName)
    {
        using var binaryWriter = new BinaryWriter(File.Open(_path + fileName, FileMode.OpenOrCreate));

        for (var i = 0; i < solution.Count; i++)
        {
            if (i % 2 != 0) binaryWriter.Write(solution[i]);
        }
    }

    public void WriteInverseProblemIteration(ReceiverLine[] receivers, double[,] phaseDifferences, double[] frequencies, string fileName)
    {
        using var streamWriterForPython = new StreamWriter(_path + fileName);

        foreach (var frequency in frequencies)
        {
            streamWriterForPython.Write($"{frequency / 1000000d} ");
        }

        streamWriterForPython.WriteLine();

        foreach (var receiver in receivers)
        {
            streamWriterForPython.Write($"{receiver.PointM.Z} ");
        }

        streamWriterForPython.WriteLine();

        for (var i = 0; i < phaseDifferences.GetLength(0); i++)
        {
            for (var j = 0; j < phaseDifferences.GetLength(1); j++)
            {
                streamWriterForPython.Write($"{phaseDifferences[i, j]} ");
            }

            streamWriterForPython.WriteLine();
        }
    }

    public void WriteInverseProblemIteration(Vector solution, string fileName)
    {
        using var streamWriterForPython = new StreamWriter(_path + fileName);

        foreach (var value in solution)
        {
            streamWriterForPython.Write($"{value} ");
        }
    }
}