namespace Application.InverseProblem;

public interface IInverseProblemContextProvider<out TInverseProblemContext>
{
    public TInverseProblemContext Get();
}