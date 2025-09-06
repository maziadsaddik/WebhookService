namespace WebhookService.Appliaction.Contract;

public class DeliveryResult
{
    public required bool Success { get; init; }
    public int? HttpStatusCode { get; init; }
    public string ErrorMessage { get; init; } = string.Empty;
    public required long DurationMs { get; init; }
}
