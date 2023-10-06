using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Dawn;
using Microsoft.EntityFrameworkCore;
using Ploch.Common.Data.Model;

namespace Ploch.Common.Data.GenericRepository.EFCore;

public class RepositoryAsync<TEntity, TId> : ReadRepositoryAsync<TEntity, TId>, IRepositoryAsync<TEntity, TId> where TEntity : class, IHasId<TId>
{
    public RepositoryAsync(DbContext dbContext) : base(dbContext)
    { }

    public async Task<TEntity> AddAsync(TEntity entity, CancellationToken cancellationToken = default)
    {
        Guard.Argument(entity, nameof(entity)).NotNull();

        await DbContext.Set<TEntity>().AddAsync(entity, cancellationToken);

        return entity;
    }

    public async Task<IEnumerable<TEntity>> AddRangeAsync(IEnumerable<TEntity> entities, CancellationToken cancellationToken = default)
    {
        Guard.Argument(entities, nameof(entities)).NotNull();

        await DbContext.AddRangeAsync(entities, cancellationToken);

        return entities;
    }

    public async Task<IEnumerable<TEntity>> AddRangeAsync(params TEntity[] entities)
    {
        Guard.Argument(entities, nameof(entities)).NotNull();

        return await AddRangeAsync(entities, CancellationToken.None);
    }

    public async Task<IEnumerable<TEntity>> AddRangeAsync(CancellationToken cancellationToken, params TEntity[] entities)
    {
        Guard.Argument(entities, nameof(entities)).NotNull();

        return await AddRangeAsync(entities, cancellationToken);
    }

    public Task DeleteAsync(TEntity entity, CancellationToken cancellationToken = default)
    {
        Guard.Argument(entity, nameof(entity)).NotNull();

        DbContext.Set<TEntity>().Remove(entity);

        return Task.CompletedTask;
    }

    public async Task UpdateAsync(TEntity entity, CancellationToken cancellationToken = default)
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