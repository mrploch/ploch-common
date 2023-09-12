using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Ploch.Common.Data.Model;

namespace Ploch.Common.Data.Repositories.Interfaces;

public interface IRepositoryAsync<TEntity, in TId> : IRepository<TEntity, TId> where TEntity : class, IHasId<TId>
{
    IQueryable<TEntity> Entities { get; }

    Task<TEntity?> GetByIdAsync(TId id, CancellationToken cancellationToken = default);

    Task<IList<TEntity>> GetAllAsync(CancellationToken cancellationToken = default);

    Task<IList<TEntity>> GetPagedResponseAsync(int pageNumber, int pageSize, CancellationToken cancellationToken = default);

    Task<TEntity> AddAsync(TEntity entity, CancellationToken cancellationToken = default);

    Task UpdateAsync(TEntity entity, CancellationToken cancellationToken = default);

    Task DeleteAsync(TEntity entity, CancellationToken cancellationToken = default);
}