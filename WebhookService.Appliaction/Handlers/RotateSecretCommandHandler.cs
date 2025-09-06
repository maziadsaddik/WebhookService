using MediatR;
using StackExchange.Redis;
using WebhookService.Appliaction.Contract;
using WebhookService.Appliaction.Contract.IRepositories;
using WebhookService.Appliaction.Dtos;
using WebhookService.Domain.Entities;

namespace WebhookService.Appliaction.Handlers
{
    public class RotateSecretCommandHandler(IUnitOfWork unitOfWork, ICryptoService cryptoService, IConnectionMultiplexer redis) : IRequestHandler<RotateSecretCommand, Subscriber>
    {
        public async Task<Subscriber> Handle(RotateSecretCommand command, CancellationToken cancellationToken)
        {
            Subscriber? subscriber = await unitOfWork.SubscriberRepository.GetByIdAsync(command.Id, cancellationToken);

            if (subscriber == null)
            {
                throw new KeyNotFoundException($"Subscriber with id {command.Id} not found.");
            }

            subscriber.RotateSecret(cryptoService.Encrypt(cryptoService.GenerateSecret()));

            await unitOfWork.SaveChangeAsync(cancellationToken);

            //await InvalidateCacheAsync(subscriber.TenantId);

            return subscriber;
        }

        private async Task InvalidateCacheAsync(string tenantId)
        {
            var db = redis.GetDatabase();

            await db.KeyDeleteAsync($"subs:{tenantId}");
        }
    }
}
