using System.Buffers;
using Application.DirectProblem;
using Application.DirectProblem._2D;
using Application.FEM._1D.BasisFunctions;
using Application.FEM._2D.Grid;
using Application.FEM.Core.BasisFunctions;
using Application.FEM.Core.Grid;
using Domain.Nodes;

namespace Application.FEM._2D.BasisFunctions;

public class BilinearBasisFunctionsProvider : IBasisFunctionsProvider<Node2D, double, IElement2D>
{
    private readonly IDirectProblemContextProvider<DirectProblem2DContext> _problemContextProvider;

    public BilinearBasisFunctionsProvider(IDirectProblemContextProvider<DirectProblem2DContext> problemContextProvider)
    {
        _problemContextProvider = problemContextProvider;
    }

    public IBasisFunction<Node2D, double>[] GetFunctions(IElement2D element)
    {
        var grid = _problemContextProvider.Get().Grid;
        var bilinearBasisFunctions = ArrayPool<IBasisFunction<Node2D, double>>.Shared.Rent(4);

        var firstXFunction = LinearFunctionsProvider.CreateFirstFunction(grid.Nodes.ElementAt(element.NodeIndexes[1]).X, element.Length);
        var secondXFunction = LinearFunctionsProvider.CreateSecondFunction(grid.Nodes.ElementAt(element.NodeIndexes[0]).X, element.Length);
        var firstYFunction = LinearFunctionsProvider.CreateFirstFunction(grid.Nodes.ElementAt(element.NodeIndexes[2]).Y, element.Height);
        var secondYFunction = LinearFunctionsProvider.CreateSecondFunction(grid.Nodes.ElementAt(element.NodeIndexes[0]).Y, element.Height);

        bilinearBasisFunctions[0] = new BasisFunction2D(firstXFunction, firstYFunction);
        bilinearBasisFunctions[1] = new BasisFunction2D(secondXFunction, firstYFunction);
        bilinearBasisFunctions[2] = new BasisFunction2D(firstXFunction, secondYFunction);
        bilinearBasisFunctions[3] = new BasisFunction2D(secondXFunction, secondYFunction);

        return bilinearBasisFunctions;
    }
}