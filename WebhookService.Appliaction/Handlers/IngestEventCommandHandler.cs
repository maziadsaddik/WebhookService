using MediatR;
using StackExchange.Redis;
using System.Text.Json;
using WebhookService.Appliaction.Contract.IRepositories;
using WebhookService.Appliaction.Dtos;
using WebhookService.Domain.Entities;

namespace WebhookService.Appliaction.Handlers
{
    public class IngestEventCommandHandler(IUnitOfWork unitOfWork, IConnectionMultiplexer redis) : IRequestHandler<IngestEventCommand, string>
    {
        public async Task<string> Handle(IngestEventCommand command, CancellationToken cancellationToken)
        {
            var dedupKey = !string.IsNullOrEmpty(command.IdempotencyKey)
                ? command.IdempotencyKey
                : $"{command.TenantId}:{command.EventType}:{Guid.NewGuid()}";

            // Check for duplicate
            var existingEvent = await unitOfWork.EventRepository
                .IsDuplicateAsync(dedupKey, cancellationToken);

            if (existingEvent)
                return $"Duplicate event detected: {dedupKey}";

            var @event = Event.Add(
                    dedupKey: dedupKey,
                    tenantId: command.TenantId,
                    eventType: command.EventType,
                    payload: JsonSerializer.Serialize(command.Payload)
            );

            await unitOfWork.EventRepository.InsertAsync(@event, cancellationToken);

            // Find matching subscribers
            var subscribers = await GetMatchingSubscribersAsync(
                command.TenantId,
                command.EventType
            );

            // Create deliveries
            foreach (var subscriber in subscribers)
            {
                await unitOfWork.DeliveryRepository.InsertAsync(
                    Delivery.CreatePending(
                        eventId: @event.Id,
                        subscriberId: subscriber.Id
                    ), cancellationToken
                );
            }

            await unitOfWork.SaveChangeAsync(cancellationToken);

            return $"Event ingested: {@event.Id} with {subscribers.Count} deliveries ";
        }

        private async Task<List<Subscriber>> GetMatchingSubscribersAsync(
            string tenantId,
            string eventType
        )
        {
            var db = redis.GetDatabase();
            var cacheKey = $"subs:{tenantId}";
            RedisValue cached = await db.StringGetAsync(cacheKey);

            IReadOnlyList<Subscriber> subscribers;

            if (!cached.IsNullOrEmpty)
            {
                var deserialized = JsonSerializer.Deserialize<List<Subscriber>>(cached.ToString());
                subscribers = deserialized ?? [];
            }
            else
            {
                subscribers = await unitOfWork.SubscriberRepository
                    .GetSubscribersByTenantIdAsync(tenantId, CancellationToken.None);

                // Cache for 60 seconds
                await db.StringSetAsync(
                    cacheKey,
                    JsonSerializer.Serialize(subscribers.Select(s => new
                    {
                        s.Id,
                        s.TenantId,
                        s.EndpointUrl,
                        s.EventTypes,
                        s.KeyId
                    })),
                    TimeSpan.FromSeconds(60));
            }

            return subscribers
                .Where(s => s.EventTypes.Contains(eventType) || s.EventTypes.Contains("*"))
                .ToList();
        }
    }
}
