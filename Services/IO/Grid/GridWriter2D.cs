using Domain.Materials;
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
    }
}
