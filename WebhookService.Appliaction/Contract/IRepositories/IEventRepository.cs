using WebhookService.Domain.Entities;

namespace WebhookService.Appliaction.Contract.IRepositories;

public interface IEventRepository : IRepository<Event, Guid>
{

}