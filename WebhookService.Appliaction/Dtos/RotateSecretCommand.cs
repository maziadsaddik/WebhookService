using MediatR;
using WebhookService.Domain.Entities;

namespace WebhookService.Appliaction.Dtos
{
    public class RotateSecretCommand : IRequest<Subscriber>
    {
        public required Guid Id { get; init; }
    }
}
