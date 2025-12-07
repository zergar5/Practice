using Application.FEM._2D;
using Domain.Edges;
using Domain.Nodes;

namespace Application.DirectProblem._2D;

public interface IDirectProblemFactory2D : IDirectProblemFactory<Node2D, IElement2D, Edge<Node2D>>;

//public class DirectProblemFactory2D : IDirectProblemFactory2D
//{
//    public IDirectProblem<T, Node2D, IElement2D, TGridParameters, Edge<Node2D>> Create<T, TGridParameters>(DirectProblemConfiguration configuration) where T : INumberBase<T>
//    {
//        if (configuration.CoordinateSystem == CoordinateSystem.Cylindrical)
//        {
//            return CreateCylindricalDirectProblem<T, TGridParameters>(configuration);
//        }

//        throw new NotImplementedException();
//    }

//    private IDirectProblem<T, Node2D, IElement2D, TGridParameters, Edge<Node2D>> CreateCylindricalDirectProblem<T, TGridParameters>(DirectProblemConfiguration configuration)
//        where T : INumberBase<T>
//    {
//        if (configuration.IsHarmonic.HasValue && configuration.IsHarmonic.Value)
//        {
//            if (configuration.IsRotor.HasValue && configuration.IsRotor.Value)
//            {
//                //return new HarmonicRotorDirectProblem2D() as IDirectProblem<T, Node2D>;

//                throw new NotImplementedException();
//            }
//        }

//        throw new NotImplementedException();
//    }
//}