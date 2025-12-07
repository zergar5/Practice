using Application.FEM._1D.BasisFunctions;

namespace Application.FEM.Core.BasisFunctions;

public class LinearFunctionsProvider
{
    public static IBasisFunction<double, double> CreateFirstFunction(double rightCoordinate, double h)
    {
        return new BasisFunction(coordinate => (rightCoordinate - coordinate) / h);
    }

    public static IBasisFunction<double, double> CreateSecondFunction(double leftCoordinate, double h)
    {
        return new BasisFunction(coordinate => (coordinate - leftCoordinate) / h);
    }
}