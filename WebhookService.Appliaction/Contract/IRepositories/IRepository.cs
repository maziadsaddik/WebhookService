using System.Linq.Expressions;
using WebhookService.Domain.Entities;

namespace WebhookService.Appliaction.Contract.IRepositories;

public interface IRepository<TEntity, TId> where TEntity : IEntity<TId>
{
    Task<TEntity?> GetByIdAsync(TId id, CancellationToken cancellationToken);

    Task<IEnumerable<TEntity>> GetAllAsync(CancellationToken cancellationToken);

    Task InsertAsync(TEntity entity, CancellationToken cancellationToken);

    Task InsertRangeAsync(IEnumerable<TEntity> entities, CancellationToken cancellationToken);

    Task DeleteAsync(TEntity entity, CancellationToken cancellationToken);

    Task DeleteRangeAsync(IReadOnlyList<TEntity> entities, CancellationToken cancellationToken);

    Task<IEnumerable<TResult>> GetAllAsync<TResult>(Expression<Func<TEntity, TResult>> target, CancellationToken cancellationToken);

    Task CanConnectAsync();
}
