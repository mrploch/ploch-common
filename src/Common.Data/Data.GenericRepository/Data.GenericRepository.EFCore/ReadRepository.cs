using System.Collections.Generic;
using System.Linq;
using Microsoft.EntityFrameworkCore;
using Ploch.Common.Data.Model;

namespace Ploch.Common.Data.GenericRepository.EFCore;

public class ReadRepository<TEntity, TId> : ReadRepositoryBase<TEntity>, IReadRepository<TEntity, TId> where TEntity : class, IHasId<TId>
{
    public ReadRepository(DbContext dbContext) : base(dbContext)
    { }

    TEntity? IReadRepository<TEntity>.GetById(object id)
    {
        return GetById((TId)id);
    }

    public IList<TEntity> GetAll()
    {
        return DbContext.Set<TEntity>().ToList();
    }

    public TEntity? GetById(TId id)
    {
        return DbContext.Set<TEntity>().Find(id);
    }

    public IList<TEntity> GetPagedResponse(int pageNumber, int pageSize)
    {
        return GetPageQuery(pageNumber, pageSize).ToList();
    }
}