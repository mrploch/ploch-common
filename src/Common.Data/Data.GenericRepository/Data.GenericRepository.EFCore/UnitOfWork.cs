﻿using System;
using System.Collections.Concurrent;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Ploch.Common.Data.Model;

namespace Ploch.Common.Data.GenericRepository.EFCore;

public class UnitOfWork : IUnitOfWork
{
    private readonly DbContext _dbContext;
    private readonly ConcurrentDictionary<string, object> _repositories = new();
    private readonly IServiceProvider _serviceProvider;
    private bool _disposed;

    public UnitOfWork(IServiceProvider serviceProvider, DbContext dbContext)
    {
        _serviceProvider = serviceProvider;
        _dbContext = dbContext ?? throw new ArgumentNullException(nameof(dbContext));
    }

    public TRepository Repository<TRepository, TEntity, TId>()
        where TRepository : IReadWriteRepositoryAsync<TEntity, TId> where TEntity : class, IHasId<TId>
    {
        var type = typeof(TEntity).Name;

        // ReSharper disable once HeapView.CanAvoidClosure - false positive
        return (TRepository)_repositories.GetOrAdd(type, _ => _serviceProvider.GetRequiredService<TRepository>());
    }

    public IReadWriteRepositoryAsync<TEntity, TId> Repository<TEntity, TId>()
        where TEntity : class, IHasId<TId>
    {
        var type = typeof(TEntity).Name;

        // ReSharper disable once HeapView.CanAvoidClosure - false positive
        return (IReadWriteRepositoryAsync<TEntity, TId>)_repositories.GetOrAdd(type, _ => _serviceProvider.GetRequiredService<IReadWriteRepositoryAsync<TEntity, TId>>());
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