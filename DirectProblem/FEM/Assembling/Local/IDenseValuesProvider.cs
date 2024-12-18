using System.Numerics;
using DirectProblem.Core.GridComponents;

namespace DirectProblem.FEM.Assembling.Local;

public interface IDenseValuesProvider<out T>
{
    public T[] GetValues(Element element);
}