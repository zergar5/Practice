using Application.FEM._2D;
using Application.FEM.Core.Grid;
using DirectProblem.Core;
using Domain.Materials;
using Domain.Nodes;
using static Application.FEM._2D.Grid.GridBuilder2D;

namespace Application.IO.Grid
{
    public class GridWriter2D
    {
        private readonly string _path;

        public GridWriter2D(string path)
        {
            _path = path;

            Directory.CreateDirectory(path);
        }

        public void WriteAreas(Grid2DParameters gridParameters, MaterialWithSigmaMu[] materials, string fileName)
        {
            using var streamWriter = new StreamWriter(_path + fileName, new FileStreamOptions { Access = FileAccess.Write, Mode = FileMode.Create });

            foreach (var area in gridParameters.Areas)
            {
                streamWriter.WriteLine($"{gridParameters.XControlPoints[area.XStartControlPointId]:F15} {gridParameters.YControlPoints[area.YStartControlPointId]:F15} " +
                                       $"{gridParameters.XControlPoints[area.XEndControlPointId]:F15} {gridParameters.YControlPoints[area.YEndControlPointId]:F15} " +
                                       $"{materials[area.MaterialId].Sigma:E15}");
            }
        }

        public void WriteMaterials(IGrid<Node2D, IElement2D> grid, string fileName)
        {
            using var binaryWriter = new BinaryWriter(File.Open(_path + fileName, FileMode.OpenOrCreate));

            foreach (var element in grid)
            {
                binaryWriter.Write(element.MaterialId + 1);
            }
        }

        public void WriteElements(IGrid<Node2D, IElement2D> grid, string fileName)
        {
            using var binaryWriter = new BinaryWriter(File.Open(_path + fileName, FileMode.OpenOrCreate));

            foreach (var element in grid)
            {
                binaryWriter.Write(element.NodeIndexes[2] + 1);
                binaryWriter.Write(element.NodeIndexes[3] + 1);
                binaryWriter.Write(element.NodeIndexes[0] + 1);
                binaryWriter.Write(element.NodeIndexes[1] + 1);
                binaryWriter.Write(0);
                binaryWriter.Write(1);
            }
        }

        public void WriteNodes(IGrid<Node2D, IElement2D> grid, string fileName)
        {
            using var binaryWriter = new BinaryWriter(File.Open(_path + fileName, FileMode.OpenOrCreate));

            foreach (var node in grid.Nodes)
            {
                binaryWriter.Write(node.R());
                binaryWriter.Write(node.Z());
            }
        }
    }
}
