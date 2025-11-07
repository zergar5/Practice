using Domain.Boundaries;
using System.Numerics;

namespace Application.FEM.Core.Assembling.Boundaries.First;

public interface IDefinedValueFirstBoundaryResolver<T, TAttachment> where T : INumberBase<T>
{
    public IFirstBoundaryValue<T>[] ResolveBoundaryValues(IDefinedValueBoundaryCondition<TAttachment, T> boundary);
}

public interface IFirstBoundaryExpressionResolver<out T, TAttachment> where T : INumberBase<T>
{
    public IFirstBoundaryValue<T>[] ResolveBoundaryValues(IBoundaryConditionExpression<TAttachment> boundary);
}