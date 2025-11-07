namespace Domain.Interfaces;

public interface IAttached<TAttachment>
{
    public TAttachment Attachment { get; set; }
}