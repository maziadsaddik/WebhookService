namespace WebhookService.Domain.Entities
{
    public class Delivery : IEntity<int>
    {
        private Delivery(
            Guid eventId,
            Guid subscriberId,
            string status,
            int attemptNumber,
            DateTime? nextRetryAt,
            string responseStatus,
            string errorMessage,
            int? httpStatusCode,
            long? durationMs,
            DateTime createdAt,
            DateTime? completedAt
        )
        {
            EventId = eventId;
            SubscriberId = subscriberId;
            Status = status;
            AttemptNumber = attemptNumber;
            NextRetryAt = nextRetryAt;
            ResponseStatus = responseStatus;
            ErrorMessage = errorMessage;
            HttpStatusCode = httpStatusCode;
            DurationMs = durationMs;
            CreatedAt = createdAt;
            CompletedAt = completedAt;
        }

        public int Id { get; }
        public Guid EventId { get; private set; }
        public Guid SubscriberId { get; private set; }
        public string Status { get; private set; } // PENDING, SUCCESS, FAILED, DLQ
        public int AttemptNumber { get; private set; }
        public DateTime? NextRetryAt { get; private set; }
        public string ResponseStatus { get; private set; }
        public string ErrorMessage { get; private set; }
        public int? HttpStatusCode { get; private set; }
        public long? DurationMs { get; private set; }
        public DateTime CreatedAt { get; private set; }
        public DateTime? CompletedAt { get; private set; }

        public Event? Event { get; private set; }
        public Subscriber? Subscriber { get; private set; }

        public static Delivery CreatePending(Guid eventId, Guid subscriberId) =>
            new(
               eventId: eventId,
               subscriberId: subscriberId,
                status: nameof(Enums.Status.Pending),
                attemptNumber: 0,
                nextRetryAt: null,
                responseStatus: string.Empty,
                errorMessage: string.Empty,
                httpStatusCode: null,
                durationMs: null,
                createdAt: DateTime.UtcNow,
                completedAt: null
            );
    }
}
