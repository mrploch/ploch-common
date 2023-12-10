using System;
using System.Threading;
using System.Threading.Tasks;
using Ploch.Common.Data.Model;

namespace Ploch.Common.Data.GenericRepository;

public interface IUnitOfWork : IDisposable
{
    IReadWriteRepositoryAsync<TEntity, TId> Repository<TEntity, TId>()
        where TEntity : class, IHasId<TId>;

    TRepository Repository<TRepository, TEntity, TId>()
        where TRepository : IReadWriteRepositoryAsync<TEntity, TId> where TEntity : class, IHasId<TId>;

    Task<int> CommitAsync(CancellationToken cancellationToken = default);

    Task RollbackAsync(CancellationToken cancellationToken = default);
}