namespace WebhookService.Domain.Entities
{
    public class Delivery : IEntity<int>
    {
        public Delivery(
            Guid eventId,
            Guid subscriberId,
            int attemptNumber,
            string status,
            int statusCode,
            string responseBody,
            DateTime? deliveredAt,
            DateTime? failedAt,
            string errorMessage
        )
        {
            EventId = eventId;
            SubscriberId = subscriberId;
            AttemptNumber = attemptNumber;
            Status = status;
            StatusCode = statusCode;
            ResponseBody = responseBody;
            DeliveredAt = deliveredAt;
            FailedAt = failedAt;
            ErrorMessage = errorMessage;
        }

        public int Id { get; }
        public Guid EventId { get; private set; }
        public Guid SubscriberId { get; private set; }
        public int AttemptNumber { get; private set; }
        public string Status { get; private set; }
        public int StatusCode { get; private set; }
        public string ResponseBody { get; private set; }
        public DateTime? DeliveredAt { get; private set; }
        public DateTime? FailedAt { get; private set; }
        public string ErrorMessage { get; private set; }

        public Event? Event { get; private set; }
        public Subscriber? Subscriber { get; private set; }
    }
}
