using System.Collections.Generic;
using System.Linq;
using Ploch.Common.Data.Model;

namespace Ploch.Common.Data.GenericRepository;


public interface IReadRepository<TEntity> : IQueryableRepository<TEntity>
    where TEntity : class
{
    TEntity? GetById(object[] keyValues);
    
    IList<TEntity> GetAll();

    IList<TEntity> GetPage(int pageNumber, int pageSize);

    int Count();
}

public interface IReadRepository<TEntity, in TId> : IReadRepository<TEntity>
    where TEntity : class, IHasId<TId>
{
    TEntity? GetById(TId id);
}