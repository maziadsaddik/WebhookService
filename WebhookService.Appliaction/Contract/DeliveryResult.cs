namespace WebhookService.Appliaction.Contract;

public class DeliveryResult
{
    public bool Success { get; set; }
    public int? HttpStatusCode { get; set; }
    public string ErrorMessage { get; set; } = string.Empty;
    public long DurationMs { get; set; }
}
