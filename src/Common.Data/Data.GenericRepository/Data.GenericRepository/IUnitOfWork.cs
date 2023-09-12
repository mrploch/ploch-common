using System;
using System.Threading;
using System.Threading.Tasks;
using Ploch.Common.Data.Model;

namespace Ploch.Common.Data.Repositories.Interfaces;

public interface IUnitOfWork : IDisposable
{
    IRepositoryAsync<TEntity, TId> Repository<TEntity, TId>() where TEntity : class, IHasId<TId>;

    Task<int> CommitAsync(CancellationToken cancellationToken = default);

    Task RollbackAsync(CancellationToken cancellationToken = default);
}