namespace Domain.Element;

public interface IElement
{
    int[] NodeIndexes { get; set; }
}

public abstract class Element : IElement
{
    public required int[] NodeIndexes { get; set; }
}