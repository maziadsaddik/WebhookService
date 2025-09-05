using MediatR;
using WebhookService.Appliaction.Dtos;
using WebhookService.Domain.Entities;

namespace WebhookService.Appliaction.Handlers
{
    public class GetDeliveriesQueryHandler(/*IUnitOfWork unitOfWork*/) : IRequestHandler<GetDeliveriesQuery, Delivery>
    {
        public async Task<Delivery> Handle(GetDeliveriesQuery query, CancellationToken cancellationToken)
        {
            throw new Exception();
        }
    }
}
