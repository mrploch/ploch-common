using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Dawn;
using Microsoft.EntityFrameworkCore;
using Ploch.Common.Data.Model;
using Ploch.Common.Data.Repositories.Interfaces;

namespace Ploch.Common.Data.Repositories.EFCore;

public abstract class Repository<TContext, TEntity, TId> : IRepository<TEntity, TId> where TEntity : class, IHasId<TId> where TContext : DbContext
{
    protected Repository(TContext dbContext)
    {
        DbContext = Guard.Argument(dbContext, nameof(dbContext)).NotNull();
    }

    protected TContext DbContext { get; }

    public IQueryable<TEntity> Entities => DbContext.Set<TEntity>();

    public TEntity Add(TEntity entity)
    {
        Guard.Argument(entity, nameof(entity)).NotNull();

        DbContext.Set<TEntity>().Add(entity);

        return entity;
    }

    public void Delete(TEntity entity)
    {
        Guard.Argument(entity, nameof(entity)).NotNull();

        DbContext.Set<TEntity>().Remove(entity);
    }

    public IList<TEntity> GetAll()
    {
        return DbContext.Set<TEntity>().ToList();
    }

    public TEntity? GetById(TId id)
    {
        return DbContext.Set<TEntity>().Find(id);
    }

    public IQueryable<TEntity> GetPageQuery(int pageNumber, int pageSize)
    {
        return DbContext.Set<TEntity>().Skip((pageNumber - 1) * pageSize).Take(pageSize).AsNoTracking();
    }

    public IList<TEntity> GetPagedResponse(int pageNumber, int pageSize)
    {
        return GetPageQuery(pageNumber, pageSize).ToList();
    }

    public void Update(TEntity entity)
    {
        Guard.Argument(entity, nameof(entity)).NotNull();

        var exist = GetById(entity.Id);
        if (exist == null)
        {
            throw new InvalidOperationException($"Entity with id {entity.Id} not found");
        }

        DbContext.Entry(exist).CurrentValues.SetValues(entity);
    }
}

public abstract class RepositoryAsync<TContext, TEntity, TId> : Repository<TContext, TEntity, TId>, IRepositoryAsync<TEntity, TId>
    where TEntity : class, IHasId<TId> where TContext : DbContext
{
    protected RepositoryAsync(TContext dbContext) : base(dbContext)
    { }

    public async Task<TEntity> AddAsync(TEntity entity, CancellationToken cancellationToken = default)
    {
        Guard.Argument(entity, nameof(entity)).NotNull();

        await DbContext.Set<TEntity>().AddAsync(entity, cancellationToken);

        return entity;
    }

    public Task DeleteAsync(TEntity entity, CancellationToken cancellationToken = default)
    {
        Delete(entity);

        return Task.CompletedTask;
    }

    public async Task<IList<TEntity>> GetAllAsync(CancellationToken cancellationToken = default)
    {
        return await Entities.ToListAsync(cancellationToken);
    }

    public async Task<TEntity?> GetByIdAsync(TId id, CancellationToken cancellationToken = default)
    {
        return await DbContext.Set<TEntity>().FindAsync(new object?[] { id }, cancellationToken);
    }

    public async Task<IList<TEntity>> GetPagedResponseAsync(int pageNumber, int pageSize, CancellationToken cancellationToken = default)
    {
        return await GetPageQuery(pageNumber, pageSize).ToListAsync(cancellationToken);
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