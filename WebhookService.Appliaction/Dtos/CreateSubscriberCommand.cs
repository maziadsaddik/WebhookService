using MediatR;
using WebhookService.Domain.Entities;

namespace WebhookService.Appliaction.Dtos
{
    public class CreateSubscriberCommand : IRequest<Subscriber>
    {
        public required string TenantId { get; init; }

        public required string WebhookUrl { get; init; }

        public required List<string> EventTypes { get; init; }
    }
}
