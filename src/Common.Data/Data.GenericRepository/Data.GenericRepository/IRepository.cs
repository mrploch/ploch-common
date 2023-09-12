using System.Collections.Generic;
using System.Linq;
using Ploch.Common.Data.Model;

namespace Ploch.Common.Data.Repositories.Interfaces;

public interface IRepository<TEntity, in TId> where TEntity : class, IHasId<TId>
{
    IQueryable<TEntity> Entities { get; }

    TEntity? GetById(TId id);

    IList<TEntity> GetAll();

    IQueryable<TEntity> GetPageQuery(int pageNumber, int pageSize);

    IList<TEntity> GetPagedResponse(int pageNumber, int pageSize);

    TEntity Add(TEntity entity);

    void Update(TEntity entity);

    void Delete(TEntity entity);
}