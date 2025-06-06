using Application.FEM._1D.BasisFunctions;
using Application.FEM.Core.BasisFunctions;
using Application.FEM.Core.Grid;
using Domain.Nodes;

namespace Application.FEM._2D.BasisFunctions;

public class BilinearBasisFunctionsProvider : IBasisFunctionsProvider<Node2D, Element2D>
{
    // TODO сетка должна доставаться из контекста или провайдера
    private readonly Grid<Node2D> _grid;

    public BilinearBasisFunctionsProvider(Grid<Node2D> grid)
    {
        _grid = grid;
    }

    public IBasisFunction<Node2D>[] GetFunctions(Element2D element)
    {
        var bilinearBasisFunctions = new BasisFunction2D[4];

        var firstXFunction = new BasisFunction(LinearFunctionsProvider.CreateFirstFunction(_grid.Nodes[element.NodeIndexes[1]].X, element.Length));
        var secondXFunction = new BasisFunction(LinearFunctionsProvider.CreateSecondFunction(_grid.Nodes[element.NodeIndexes[0]].X, element.Length));
        var firstYFunction = new BasisFunction(LinearFunctionsProvider.CreateFirstFunction(_grid.Nodes[element.NodeIndexes[2]].Y, element.Height));
        var secondYFunction = new BasisFunction(LinearFunctionsProvider.CreateSecondFunction(_grid.Nodes[element.NodeIndexes[0]].Y, element.Height));

        bilinearBasisFunctions[0] = new BasisFunction2D(firstXFunction, firstYFunction);
        bilinearBasisFunctions[1] = new BasisFunction2D(secondXFunction, firstYFunction);
        bilinearBasisFunctions[2] = new BasisFunction2D(firstXFunction, secondYFunction);
        bilinearBasisFunctions[3] = new BasisFunction2D(secondXFunction, secondYFunction);

        return bilinearBasisFunctions;
    }
}