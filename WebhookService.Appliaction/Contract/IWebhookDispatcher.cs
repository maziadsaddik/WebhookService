using WebhookService.Domain.Entities;

namespace WebhookService.Appliaction.Contract;

public interface IWebhookDispatcher
{
    Task<DeliveryResult> DispatchAsync(Subscriber subscriber, Event evt, Delivery delivery);
}
