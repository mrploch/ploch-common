using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.EntityFrameworkCore;
using Ploch.Common.Data.Model;

namespace Ploch.Common.Data.GenericRepository.EFCore;

public class ReadRepository<TEntity> : QueryableRepository<TEntity>, IReadRepository<TEntity>
    where TEntity : class
{
    public TEntity? GetById(object[] keyValues)
    {
        return DbSet.Find(keyValues);
    }
    
    public IList<TEntity> GetAll()
    {
        return Entities.ToList();
    }

    public IList<TEntity> GetPage(int pageNumber, int pageSize)
    {
        return GetPageQuery(pageNumber, pageSize).ToList();
    }

    public int Count()
    {
        return Entities.Count();
    }

    public ReadRepository(DbContext dbContext) : base(dbContext)
    {
    }
}

public class ReadRepository<TEntity, TId> : ReadRepository<TEntity>, IReadRepository<TEntity, TId>
    where TEntity : class, IHasId<TId>
{
    public ReadRepository(DbContext dbContext) : base(dbContext)
    { }

 
    public TEntity? GetById(TId id)
    {
        return DbSet.Find(id);
    }
}