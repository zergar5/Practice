namespace Domain.Environment;

public class ReceiverLine<TLocation>
{
    public required TLocation ReceiverM { get; set; }
    public required TLocation ReceiverN { get; set; }
}