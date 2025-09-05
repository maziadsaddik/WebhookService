using MediatR;
using WebhookService.Appliaction.Contract;
using WebhookService.Appliaction.Contract.IRepositories;
using WebhookService.Appliaction.Dtos;

namespace WebhookService.Appliaction.Handlers
{
    public class IngestEventCommandHandler(IUnitOfWork unitOfWork, ICryptoService cryptoService) : IRequestHandler<IngestEventCommand, string>
    {
        public async Task<string> Handle(IngestEventCommand command, CancellationToken cancellationToken)
        {
            //    var dedupKey = !string.IsNullOrEmpty(command.IdempotencyKey)
            //        ? command.IdempotencyKey
            //        : $"{command.TenantId}:{command.EventType}:{Guid.NewGuid()}";

            //    // Check for duplicate
            //    var existingEvent = await unitOfWork.EventRepository
            //        .FirstOrDefaultAsync(e => e.DedupKey == dedupKey);

            //    if (existingEvent != null)
            //    {
            //        _logger.LogInformation("Duplicate event detected: {DedupKey}", dedupKey);
            //        return existingEvent;
            //    }

            //    var evt = new Event
            //    {
            //        Id = Guid.NewGuid(),
            //        TenantId = request.TenantId,
            //        EventType = request.EventType,
            //        Payload = JsonSerializer.Serialize(request.Payload),
            //        DedupKey = dedupKey,
            //        CreatedAt = DateTime.UtcNow
            //    };

            //    _db.Events.Add(evt);

            //    // Find matching subscribers
            //    var subscribers = await GetMatchingSubscribersAsync(
            //        request.TenantId,
            //        request.EventType);

            //    // Create deliveries
            //    foreach (var subscriber in subscribers)
            //    {
            //        var delivery = new Delivery
            //        {
            //            Id = Guid.NewGuid(),
            //            EventId = evt.Id,
            //            SubscriberId = subscriber.Id,
            //            Status = "PENDING",
            //            AttemptNumber = 0,
            //            CreatedAt = DateTime.UtcNow
            //        };
            //        _db.Deliveries.Add(delivery);
            //    }

            //    await _db.SaveChangesAsync();

            //    _logger.LogInformation(
            //        "Event ingested: {EventId} with {DeliveryCount} deliveries",
            //        evt.Id, subscribers.Count);

            //    return evt;


            return string.Empty;
        }

        //private async Task<List<Subscriber>> GetMatchingSubscribersAsync(
        //string tenantId,
        //string eventType)
        //{
        //    var db = _redis.GetDatabase();
        //    var cacheKey = $"subs:{tenantId}";
        //    var cached = await db.StringGetAsync(cacheKey);

        //    List<Subscriber> subscribers;

        //    if (!cached.IsNullOrEmpty)
        //    {
        //        subscribers = JsonSerializer.Deserialize<List<Subscriber>>(cached);
        //    }
        //    else
        //    {
        //        subscribers = await _db.Subscribers
        //            .Where(s => s.TenantId == tenantId && s.IsActive)
        //            .ToListAsync();

        //        // Cache for 60 seconds
        //        await db.StringSetAsync(
        //            cacheKey,
        //            JsonSerializer.Serialize(subscribers.Select(s => new
        //            {
        //                s.Id,
        //                s.TenantId,
        //                s.EndpointUrl,
        //                s.EventTypes,
        //                s.KeyId
        //            })),
        //            TimeSpan.FromSeconds(60));
        //    }

        //    return subscribers
        //        .Where(s => s.EventTypes.Contains(eventType) || s.EventTypes.Contains("*"))
        //        .ToList();
        //}
    }
}
