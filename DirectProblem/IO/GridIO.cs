﻿using DirectProblem.Core;
using DirectProblem.Core.Base;
using DirectProblem.Core.GridComponents;

namespace DirectProblem.IO
{
    public class GridIO
    {
        private readonly string _path;
        public GridIO(string path)
        {
            _path = path;
        }

        public void WriteMaterials(Grid<Node2D> grid, string fileName)
        {
            using var binaryWriter = new BinaryWriter(File.Open(_path + fileName, FileMode.OpenOrCreate));

            foreach (var element in grid)
            {
                binaryWriter.Write(element.MaterialId + 1);
            }
        }

        public void WriteElements(Grid<Node2D> grid, string fileName)
        {
            using var binaryWriter = new BinaryWriter(File.Open(_path + fileName, FileMode.OpenOrCreate));

            foreach (var element in grid)
            {
                binaryWriter.Write(element.NodesIndexes[2] + 1);
                binaryWriter.Write(element.NodesIndexes[3] + 1);
                binaryWriter.Write(element.NodesIndexes[0] + 1);
                binaryWriter.Write(element.NodesIndexes[1] + 1);
                binaryWriter.Write(0);
                binaryWriter.Write(1);
            }
        }

        public void WriteNodes(Grid<Node2D> grid, string fileName)
        {
            using var binaryWriter = new BinaryWriter(File.Open(_path + fileName, FileMode.OpenOrCreate));

            for (var i = 0; i < grid.Nodes.ZLength; i++)
            {
                for (var j = 0; j < grid.Nodes.RLength; j++)
                {
                    var node = grid.Nodes[i * grid.Nodes.RLength + j];

                    binaryWriter.Write(node.R);
                    binaryWriter.Write(node.Z);
                }
            }
        }

        public void WriteAreas(Grid<Node2D> grid, Vector sigmas, string fileName)
        {
            using var streamWriter = new StreamWriter(_path + fileName);

            foreach (var area in grid.Areas)
            {
                streamWriter.WriteLine($"{area.LowerLeftCorner.R} {area.LowerLeftCorner.Z} " +
                                       $"{area.UpperRightCorner.R} {area.UpperRightCorner.Z} " +
                                       $"{sigmas[area.MaterialId]}");
            }
        }
    }
}
