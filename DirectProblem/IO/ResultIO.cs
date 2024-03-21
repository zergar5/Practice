using DirectProblem.Core;
using DirectProblem.Core.Base;
using DirectProblem.Core.GridComponents;
using System.Drawing;
using System.Numerics;
using System.Reflection;
using System.Xml.Linq;
using Vector = DirectProblem.Core.Base.Vector;

namespace DirectProblem.IO;

public class ResultIO
{
    private readonly string _path;

    public ResultIO(string path)
    {
        _path = path;
    }

    public void Write(string fileName, double[] omegas, double[] centersZ, Complex[,] emfs)
    {
        using var streamWriter = new StreamWriter(_path + fileName);

        foreach (var omega in omegas)
        {
            streamWriter.Write($"{omega / 1000000d} ");
        }

        streamWriter.WriteLine();

        foreach (var centerZ in centersZ)
        {
            streamWriter.Write($"{centerZ} ");
        }

        streamWriter.WriteLine();

        for (var i = 0; i < emfs.GetLength(1); i++)
        {
            for (var j = 0; j < emfs.GetLength(0); j++)
            {
                streamWriter.Write($"{emfs[j, i].Real} ");
            }

            streamWriter.WriteLine();
        }

        for (var i = 0; i < emfs.GetLength(1); i++)
        {
            for (var j = 0; j < emfs.GetLength(0); j++)
            {
                streamWriter.Write($"{emfs[j, i].Imaginary} ");
            }

            streamWriter.WriteLine();
        }
    }

    public void Write(string fileName, double[] omegas, double[] centersZ, double[,] phaseDifferences)
    {
        using var streamWriter = new StreamWriter(_path + fileName);

        foreach (var omega in omegas)
        {
            streamWriter.Write($"{omega / 1000000d} ");
        }

        streamWriter.WriteLine();

        foreach (var centerZ in centersZ)
        {
            streamWriter.Write($"{centerZ} ");
        }

        streamWriter.WriteLine();

        for (var i = 0; i < phaseDifferences.GetLength(1); i++)
        {
            for (var j = 0; j < phaseDifferences.GetLength(0); j++)
            {
                streamWriter.Write($"{phaseDifferences[j, i]} ");
            }

            streamWriter.WriteLine();
        }
    }

    public void WriteSinuses(Vector solution, string fileName)
    {
        using var binaryWriter = new BinaryWriter(File.Open(_path + fileName, FileMode.OpenOrCreate));

        for (var i = 0; i < solution.Count; i++)
        {
            if(i % 2 == 0) binaryWriter.Write(solution[i]);
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
}