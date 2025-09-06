using Microsoft.EntityFrameworkCore;
using System.Linq.Expressions;
using WebhookService.Appliaction.Contract.IRepositories;
using WebhookService.Domain.Entities;

namespace WebhookService.Infrastructure.Persistence.Repositories
{
    public class Repository<TEntity, TId>(AppDbContext dbContext) : IRepository<TEntity, TId>
    where TEntity : class, IEntity<TId>
    {
        public virtual Task DeleteAsync(TEntity entity, CancellationToken cancellationToken)
        {
            dbContext.Remove(entity);

            return Task.CompletedTask;
        }

        public virtual async Task DeleteRangeAsync(IReadOnlyList<TEntity> entities, CancellationToken cancellationToken)
        {
            dbContext.Set<TEntity>().RemoveRange(entities);

            await Task.CompletedTask;
        }

        public virtual Task<TEntity?> GetByIdAsync(TId id, CancellationToken cancellationToken)
             => dbContext.Set<TEntity>().FindAsync(keyValues: [id], cancellationToken: cancellationToken).AsTask();

        public virtual Task InsertAsync(TEntity entity, CancellationToken cancellationToken)
            => dbContext.Set<TEntity>().AddAsync(entity, cancellationToken).AsTask();

        public virtual Task InsertRangeAsync(IEnumerable<TEntity> entity, CancellationToken cancellationToken)
            => dbContext.Set<TEntity>().AddRangeAsync(entity, cancellationToken);

        public virtual Task UpsertAsync(TEntity entity, CancellationToken cancellationToken)
        {
            var existingEntity = dbContext.Set<TEntity>().FindAsync(keyValues: [entity.Id], cancellationToken: cancellationToken).AsTask();

            if (existingEntity == null)
            {
                dbContext.Set<TEntity>().AddAsync(entity);
            }
            else
            {
                dbContext.Set<TEntity>().Update(entity);
            }

            return Task.CompletedTask;
        }

        public virtual async Task<IEnumerable<TEntity>> GetAllAsync(CancellationToken cancellationToken)
            => await dbContext.Set<TEntity>().AsNoTracking()
                                                     .ToListAsync(cancellationToken);

        public virtual async Task<IEnumerable<TResult>> GetAllAsync<TResult>(Expression<Func<TEntity, TResult>> target, CancellationToken cancellationToken) => await dbContext.Set<TEntity>().AsNoTracking()
                                                     .Select(target)
                                                     .ToListAsync(cancellationToken);
        public virtual async Task CanConnectAsync()
            => await dbContext.Database.CanConnectAsync();
    }
}
