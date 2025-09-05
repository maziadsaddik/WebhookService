using WebhookService.Appliaction.Contract.IRepositories;

namespace WebhookService.Infrastructure.Persistence.Repositories
{
    public class UnitOfWork(AppDbContext dbContext) : IUnitOfWork
    {
        public ISubscriberRepository SubscriberRepository => new SubscriberRepository(dbContext);
        public IDeliveryRepository DeliveryRepository => new DeliveryRepository(dbContext);
        public IEventRepository EventRepository => new EventRepository(dbContext);

        public Task SaveChangeAsync(CancellationToken cancellationToken)
            => dbContext.SaveChangesAsync(cancellationToken);

        public void Dispose()
        {
            dbContext.Dispose();

            GC.SuppressFinalize(this);
        }
    }
}
