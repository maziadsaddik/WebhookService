namespace WebhookService.API.Models.Inputs
{
    public class CreateSubscriberInput
    {
        public required string TenantId { get; init; }
        public required string WebhookUrl { get; init; }
        public required List<string> EventTypes { get; init; }
    }
}
