namespace SharedKernel.Events;

public class StoredEvent
{
    public Guid Id { get; private set; }
    public string Type { get; private set; }
    public string Content { get; private set; }
    public DateTime OccurredOn { get; private set; }
    public string TenantId { get; private set; }
    public bool Processed { get; private set; }
    public DateTime? ProcessedOn { get; private set; }

    public int RetryCount { get; private set; }
    public string? LastError { get; private set; }

    public StoredEvent(Guid id, string type, string content, string tenantId)
    {
        Id = id;
        Type = type;
        Content = content;
        TenantId = tenantId;
        OccurredOn = DateTime.UtcNow;
        Processed = false;
        RetryCount = 0;
        LastError = null;
    }

    public void MarkAsProcessed()
    {
        Processed = true;
        ProcessedOn = DateTime.UtcNow;
    }

    public void RecordFailure(string errorMessage)
    {
        RetryCount++;
        LastError = errorMessage;
    }
}