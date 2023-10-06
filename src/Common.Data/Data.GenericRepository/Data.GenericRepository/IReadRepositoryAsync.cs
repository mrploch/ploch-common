using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Ploch.Common.Data.Model;

namespace Ploch.Common.Data.GenericRepository;

public interface IReadRepositoryAsync<TEntity, in TId> where TEntity : class, IHasId<TId>
{
    Task<TEntity?> GetByIdAsync(TId id, CancellationToken cancellationToken = default);

    Task<IList<TEntity>> GetAllAsync(CancellationToken cancellationToken = default);

    Task<IList<TEntity>> GetPagedResponseAsync(int pageNumber, int pageSize, CancellationToken cancellationToken = default);
}