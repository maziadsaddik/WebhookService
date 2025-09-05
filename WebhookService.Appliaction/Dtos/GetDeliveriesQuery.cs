using MediatR;
using WebhookService.Domain.Entities;

namespace WebhookService.Appliaction.Dtos
{
    public class GetDeliveriesQuery : IRequest<(IReadOnlyList<Delivery>, int)>
    {
        public required Guid? EventId { get; init; }
        public required Guid? SubscriberId { get; init; }
        public required string Status { get; init; }
        public required int CurrentPage { get; init; }
        public required int PageSize { get; init; }
    }
}
