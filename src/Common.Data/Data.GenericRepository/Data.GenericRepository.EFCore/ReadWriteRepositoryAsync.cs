using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Dawn;
using Microsoft.EntityFrameworkCore;
using Ploch.Common.Data.Model;

namespace Ploch.Common.Data.GenericRepository.EFCore;

public class ReadWriteRepositoryAsync<TEntity, TId> : ReadRepositoryAsync<TEntity, TId>, IReadWriteRepositoryAsync<TEntity, TId>
    where TEntity : class, IHasId<TId>
{
    public ReadWriteRepositoryAsync(DbContext dbContext) : base(dbContext)
    { }

    public virtual async Task<TEntity> AddAsync(TEntity entity, CancellationToken cancellationToken = default)
    {
        Guard.Argument(entity, nameof(entity)).NotNull();

        await DbContext.Set<TEntity>().AddAsync(entity, cancellationToken);

        return entity;
    }

    public virtual async Task<IEnumerable<TEntity>> AddRangeAsync(IEnumerable<TEntity> entities, CancellationToken cancellationToken = default)
    {
        Guard.Argument(entities, nameof(entities)).NotNull();

        await DbContext.AddRangeAsync(entities, cancellationToken);

        return entities;
    }

    public virtual async Task<IEnumerable<TEntity>> AddRangeAsync(params TEntity[] entities)
    {
        Guard.Argument(entities, nameof(entities)).NotNull();

        return await AddRangeAsync(entities, CancellationToken.None);
    }

    public virtual async Task<IEnumerable<TEntity>> AddRangeAsync(CancellationToken cancellationToken, params TEntity[] entities)
    {
        Guard.Argument(entities, nameof(entities)).NotNull();

        return await AddRangeAsync(entities, cancellationToken);
    }

    public virtual Task DeleteAsync(TEntity entity, CancellationToken cancellationToken = default)
    {
        Guard.Argument(entity, nameof(entity)).NotNull();

        DbContext.Set<TEntity>().Remove(entity);

        return Task.CompletedTask;
    }

    public virtual async Task UpdateAsync(TEntity entity, CancellationToken cancellationToken = default)
    {
        Guard.Argument(entity, nameof(entity)).NotNull();

        var exist = await GetByIdAsync(entity.Id, cancellationToken);
        if (exist == null)
        {
            throw new InvalidOperationException($"Entity with id {entity.Id} not found");
        }

        DbContext.Entry(exist).CurrentValues.SetValues(entity);
    }
}