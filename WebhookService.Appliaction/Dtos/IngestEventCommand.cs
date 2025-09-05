using MediatR;

namespace WebhookService.Appliaction.Dtos
{
    public class IngestEventCommand : IRequest<string>
    {
        public required string TenantId { get; init; }
        public required string EventType { get; init; }
        public required object Payload { get; init; }
        public required string IdempotencyKey { get; init; }
    }
}
