using MediatR;
using WebhookService.Appliaction.Contract.IRepositories;
using WebhookService.Appliaction.Dtos;
using WebhookService.Domain.Entities;

namespace WebhookService.Appliaction.Handlers
{
    public class GetDeliveriesQueryHandler(IUnitOfWork unitOfWork) : IRequestHandler<GetDeliveriesQuery, (IReadOnlyList<Delivery>, int)>
    {
        public async Task<(IReadOnlyList<Delivery>, int)> Handle(GetDeliveriesQuery query, CancellationToken cancellationToken)
        {
            return await unitOfWork.DeliveryRepository.GetDeliveriesWithPaginationAsync(
                query: query,
                cancellationToken: cancellationToken
            );
        }
    }
}
