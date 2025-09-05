using MediatR;
using WebhookService.Appliaction.Contract;
using WebhookService.Appliaction.Contract.IRepositories;
using WebhookService.Appliaction.Dtos;
using WebhookService.Domain.Entities;

namespace WebhookService.Appliaction.Handlers
{
    public class CreateSubscriberCommandHandler(IUnitOfWork unitOfWork, ICryptoService cryptoService) : IRequestHandler<CreateSubscriberCommand, Subscriber>
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

            return subscriber;
        }
    }
}
