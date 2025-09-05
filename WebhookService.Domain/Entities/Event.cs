namespace WebhookService.Domain.Entities
{
    public class Event : IEntity<Guid>
    {
        private Event(
            string eventType,
            string tenantId,
            string payload,
            DateTime receivedAt
        )
        {
            EventType = eventType;
            TenantId = tenantId;
            Payload = payload;
            ReceivedAt = receivedAt;
        }

        public Guid Id { get; }
        public string EventType { get; private set; }
        public string TenantId { get; private set; }
        public string Payload { get; private set; }
        public DateTime ReceivedAt { get; private set; }

        public virtual ICollection<Delivery> Deliveries { get; } = new List<Delivery>();
    }
}
