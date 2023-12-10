using System.Collections.Generic;
using Ploch.Common.Data.Model;

namespace Ploch.Common.Data.GenericRepository;

public interface IWriteRepository<TEntity, in TId>
    where TEntity : class, IHasId<TId>
{
    TEntity Add(TEntity entity);

    IEnumerable<TEntity> AddRange(IEnumerable<TEntity> entities);

    void Update(TEntity entity);

    void Delete(TEntity entity);
}