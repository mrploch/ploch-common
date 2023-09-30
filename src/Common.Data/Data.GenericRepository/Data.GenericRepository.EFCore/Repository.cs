using System;
using System.Collections.Generic;
using Dawn;
using Microsoft.EntityFrameworkCore;
using Ploch.Common.Data.Model;

namespace Ploch.Common.Data.GenericRepository.EFCore;

public class Repository<TEntity, TId> : ReadRepository<TEntity, TId>, IRepository<TEntity, TId> where TEntity : class, IHasId<TId>
{
    public Repository(DbContext dbContext) : base(dbContext)
    { }

    public TEntity Add(TEntity entity)
    {
        Guard.Argument(entity, nameof(entity)).NotNull();

        DbContext.Set<TEntity>().Add(entity);

        return entity;
    }

    public IEnumerable<TEntity> AddRange(IEnumerable<TEntity> entities)
    {
        Guard.Argument(entities, nameof(entities)).NotNull();

        DbContext.AddRange(entities);

        return entities;
    }

    public void Delete(TEntity entity)
    {
        Guard.Argument(entity, nameof(entity)).NotNull();

        DbContext.Set<TEntity>().Remove(entity);
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