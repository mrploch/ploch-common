using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Ploch.Common.Data.Model;

namespace Ploch.Common.Data.GenericRepository;

public interface IReadRepositoryAsync<TEntity> : IQueryableRepository<TEntity>
    where TEntity : class
{
    Task<TEntity> GetByIdAsync(object[] keyValues, CancellationToken cancellationToken = default);
    
    Task<IList<TEntity>> GetAllAsync(CancellationToken cancellationToken = default);

    Task<IList<TEntity>> GetPageAsync(int pageNumber, int pageSize, CancellationToken cancellationToken = default);

    Task<int> GetCountAsync(CancellationToken cancellationToken = default);
}

public interface IReadRepositoryAsync<TEntity, in TId> : IReadRepositoryAsync<TEntity>
    where TEntity : class, IHasId<TId>
{
    Task<TEntity?> GetByIdAsync(TId id, CancellationToken cancellationToken = default);
}