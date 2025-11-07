using Application.FEM.Core.Grid;
using Domain.Boundaries;
using Domain.Environment;
using System.Numerics;
using Domain.Nodes;

namespace Application.DirectProblem;

public interface IDirectProblemContextProvider<out TDirectProblemContext>
{
    public TDirectProblemContext Get();
}