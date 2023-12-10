using System.Collections.Generic;
using System.Linq;
using Ploch.Common.Data.Model;

namespace Ploch.Common.Data.GenericRepository;

public interface IReadRepository<TEntity>
    where TEntity : class
{
    IQueryable<TEntity> Entities { get; }

    TEntity? GetById(object id);

    IList<TEntity> GetAll();

    IQueryable<TEntity> GetPageQuery(int pageNumber, int pageSize);

    IList<TEntity> GetPagedResponse(int pageNumber, int pageSize);

    int Count();
}

public interface IReadRepository<TEntity, in TId> : IReadRepository<TEntity>
    where TEntity : class, IHasId<TId>
{
    TEntity? GetById(TId id);
}