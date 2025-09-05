namespace WebhookService.Domain.Entities
{
    public class Subscriber : IEntity<Guid>
    {
        private Subscriber(
            string tenantId,
            string endpointUrl,
            List<string> eventTypes,
            string encryptedSecret,
            string keyId,
            bool isActive,
            DateTime createdAt,
            DateTime? updatedAt
        )
        {
            TenantId = tenantId;
            EndpointUrl = endpointUrl;
            EventTypes = eventTypes;
            EncryptedSecret = encryptedSecret;
            KeyId = keyId;
            IsActive = isActive;
            CreatedAt = createdAt;
            UpdatedAt = updatedAt;
        }

        public Guid Id { get; }
        public string TenantId { get; private set; }
        public string EndpointUrl { get; private set; }
        public List<string> EventTypes { get; private set; }
        public string EncryptedSecret { get; private set; }
        public string KeyId { get; private set; }
        public bool IsActive { get; private set; }
        public DateTime CreatedAt { get; private set; }
        public DateTime? UpdatedAt { get; private set; }


        public static Subscriber Create(string tenantId, string endpointUrl, List<string> eventTypes, string secret) => new(
            tenantId: tenantId,
            endpointUrl: endpointUrl,
            eventTypes: eventTypes,
            encryptedSecret: secret,
            keyId: Guid.NewGuid().ToString(),
            isActive: true,
            createdAt: DateTime.UtcNow,
            updatedAt: null
        );

        public void RotateSecret(string secret)
        {
            EncryptedSecret = secret;
            KeyId = Guid.NewGuid().ToString();
            UpdatedAt = DateTime.UtcNow;
        }

        public virtual ICollection<Delivery> Deliveries { get; } = new List<Delivery>();
    }
}
