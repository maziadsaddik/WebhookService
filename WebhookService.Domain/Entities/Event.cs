namespace WebhookService.Domain.Entities
{
    public class Event : IEntity<Guid>
    {
        private Event(
            Guid id,
            string eventType,
            string tenantId,
            string payload,
            string dedupKey,
            DateTime receivedAt
        )
        {
            Id = id;
            EventType = eventType;
            TenantId = tenantId;
            Payload = payload;
            DedupKey = dedupKey;
            ReceivedAt = receivedAt;
        }

        public Guid Id { get; private set; }
        public string EventType { get; private set; }
        public string TenantId { get; private set; }
        public string Payload { get; private set; }
        public string DedupKey { get; private set; }
        public DateTime ReceivedAt { get; private set; }

        public virtual ICollection<Delivery> Deliveries { get; } = new List<Delivery>();

        public static Event Add(
            string dedupKey,
            string eventType,
            string tenantId,
            string payload
        ) => new(
           id: Guid.NewGuid(),
           eventType: eventType,
           tenantId: tenantId,
           payload: payload,
           dedupKey: dedupKey,
           receivedAt: DateTime.UtcNow
        );
    }
}
