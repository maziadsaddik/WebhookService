using MediatR;

namespace WebhookService.Appliaction.Dtos
{
    public class SubscriberStatusQuery : IRequest<object>
    {
        public required Guid Id { get; init; }
    }
}
