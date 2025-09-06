using MediatR;
using StackExchange.Redis;
using WebhookService.Appliaction.Contract;
using WebhookService.Appliaction.Contract.IRepositories;
using WebhookService.Appliaction.Dtos;
using WebhookService.Domain.Entities;

namespace WebhookService.Appliaction.Handlers
{
    public class CreateSubscriberCommandHandler(IUnitOfWork unitOfWork, ICryptoService cryptoService, IConnectionMultiplexer redis) : IRequestHandler<CreateSubscriberCommand, Subscriber>
    {
        public async Task<Subscriber> Handle(CreateSubscriberCommand command, CancellationToken cancellationToken)
        {
            Subscriber subscriber = Subscriber.Create(
               tenantId: command.TenantId,
               endpointUrl: command.WebhookUrl,
               eventTypes: command.EventTypes,
               cryptoService.Encrypt(cryptoService.GenerateSecret())
            );

            await unitOfWork.SubscriberRepository.InsertAsync(subscriber, cancellationToken);

            await unitOfWork.SaveChangeAsync(cancellationToken);

            await InvalidateCacheAsync(command.TenantId);

            return subscriber;
        }

        private async Task InvalidateCacheAsync(string tenantId)
        {
            var db = redis.GetDatabase();

            await db.KeyDeleteAsync($"subs:{tenantId}");
        }
    }
}
