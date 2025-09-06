using MediatR;
using StackExchange.Redis;
using WebhookService.Appliaction.Contract.IRepositories;
using WebhookService.Appliaction.Dtos;
using WebhookService.Domain.Enums;

namespace WebhookService.Appliaction.Handlers
{
    public class SubscriberStatusQueryHandler(IUnitOfWork unitOfWork, IConnectionMultiplexer redis) : IRequestHandler<SubscriberStatusQuery, object>
    {
        public async Task<object> Handle(SubscriberStatusQuery query, CancellationToken cancellationToken)
        {
            Domain.Entities.Subscriber? subscriber = await unitOfWork.SubscriberRepository.GetSubscriberById(
                query.Id,
                cancellationToken
            );

            if (subscriber is null)
            {
                return new { Message = "Subscriber not found" };
            }

            List<Domain.Entities.Delivery> recentDeliveries = subscriber.Deliveries
            .OrderByDescending(d => d.CreatedAt)
            .Take(10)
            .ToList();

            return new
            {
                subscriber.Id,
                subscriber.EndpointUrl,
                subscriber.IsActive,
                subscriber.EventTypes,
                Stats = new
                {
                    TotalDeliveries = subscriber.Deliveries.Count,
                    SuccessfulDeliveries = subscriber.Deliveries.Count(d => d.Status == nameof(Status.Success)),
                    FailedDeliveries = subscriber.Deliveries.Count(d => d.Status == nameof(Status.Failed)),
                    DlqDeliveries = subscriber.Deliveries.Count(d => d.Status == nameof(Status.Dlq))
                },
                RecentDeliveries = recentDeliveries.Select(d => new
                {
                    d.EventId,
                    d.Status,
                    d.AttemptNumber,
                    d.HttpStatusCode,
                    d.CreatedAt
                })
            };
        }
    }
}
