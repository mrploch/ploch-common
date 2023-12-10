using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Ploch.Common.Data.Model;

namespace Ploch.Common.Data.GenericRepository;

public interface IWriteRepositoryAsync<TEntity, in TId>
    where TEntity : class, IHasId<TId>
{
    Task<TEntity> AddAsync(TEntity entity, CancellationToken cancellationToken = default);

    Task<IEnumerable<TEntity>> AddRangeAsync(IEnumerable<TEntity> entities, CancellationToken cancellationToken = default);

    Task<IEnumerable<TEntity>> AddRangeAsync(params TEntity[] entities);

    Task<IEnumerable<TEntity>> AddRangeAsync(CancellationToken cancellationToken, params TEntity[] entities);

    Task UpdateAsync(TEntity entity, CancellationToken cancellationToken = default);

    Task DeleteAsync(TEntity entity, CancellationToken cancellationToken = default);
}