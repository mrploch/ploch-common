using System;
using System.Collections.Concurrent;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Ploch.Common.Data.Model;
using Ploch.Common.Data.Repositories.Interfaces;

namespace Ploch.Common.Data.Repositories.EFCore;

public abstract class UnitOfWork<TContext> : IUnitOfWork where TContext : DbContext
{
    private readonly TContext _dbContext;
    private readonly ConcurrentDictionary<string, object> _repositories = new();
    private readonly IServiceProvider _serviceProvider;
    private bool _disposed;

    protected UnitOfWork(IServiceProvider serviceProvider, TContext dbContext)
    {
        _serviceProvider = serviceProvider;
        _dbContext = dbContext ?? throw new ArgumentNullException(nameof(dbContext));
    }

    public IRepositoryAsync<TEntity, TId> Repository<TEntity, TId>() where TEntity : class, IHasId<TId>
    {
        var type = typeof(TEntity).Name;

        // ReSharper disable once HeapView.CanAvoidClosure - false positive
        return (IRepositoryAsync<TEntity, TId>)_repositories.GetOrAdd(type, _ => _serviceProvider.GetRequiredService<IRepositoryAsync<TEntity, TId>>());

        /*if (!_repositories.ContainsKey(type))
        {
            // var repositoryType = typeof(RepositoryAsync<,,>);
            // var repositoryInstance = Activator.CreateInstance(repositoryType.MakeGenericType(typeof(TContext), typeof(TEntity), typeof(TId)), _dbContext);
            var repositoryInstance = _serviceProvider.GetRequiredService<IRepositoryAsync<TEntity, TId>>();

            _repositories.Add(type, repositoryInstance);
        }

        return (IRepositoryAsync<TEntity, TId>)_repositories[type]!;*/
    }

    public async Task<int> CommitAsync(CancellationToken cancellationToken = default)
    {
        return await _dbContext.SaveChangesAsync(cancellationToken);
    }

    public Task RollbackAsync(CancellationToken cancellationToken = default)
    {
        _dbContext.ChangeTracker.Entries().ToList().ForEach(x => x.Reload());

        return Task.CompletedTask;
    }

    public void Dispose()
    {
        Dispose(true);
        GC.SuppressFinalize(this);
    }

    protected virtual void Dispose(bool disposing)
    {
        if (!_disposed && disposing)
        {
            _dbContext.Dispose();
        }

        _disposed = true;
    }
}