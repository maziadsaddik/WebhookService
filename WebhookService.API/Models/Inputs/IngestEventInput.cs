namespace WebhookService.API.Models.Inputs
{
    public class IngestEventInput
    {
        public required string TenantId { get; init; }
        public required string EventType { get; init; }
        public required object Payload { get; init; }
    }
}
