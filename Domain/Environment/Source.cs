namespace Domain.Environment;

public class Source<TLocation>
{
    public required TLocation Location { get; set; }
    public required double Power { get; set; }
}