namespace Application.DirectProblem;

public interface IDirectProblemContextProvider<out TDirectProblemContext>
{
    public TDirectProblemContext Get();
}