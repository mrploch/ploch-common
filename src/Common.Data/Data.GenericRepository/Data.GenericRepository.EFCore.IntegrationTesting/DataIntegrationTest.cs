﻿using Microsoft.Data.Sqlite;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Ploch.Common.Data.Model;

namespace Ploch.Common.Data.GenericRepository.EFCore.IntegrationTesting;

/// <summary>
///     Base class for integration tests that use EF Core in-memory SQLite database.
/// </summary>
/// <typeparam name="TDbContext">The data context type.</typeparam>
public abstract class DataIntegrationTest<TDbContext> : IDisposable
    where TDbContext : DbContext
{
    protected DataIntegrationTest(string connectionString = "Filename=:memory:")
    {
        var serviceCollection = new ServiceCollection();

        ConfigureServices(serviceCollection);

        serviceCollection.AddDbContext<TDbContext>(builder =>
                                                   {
                                                       var connection = new SqliteConnection(connectionString);
                                                       connection.Open();
                                                       builder.UseSqlite(connection);
                                                   });
        serviceCollection.AddRepositories<TDbContext>();
        ServiceProvider = serviceCollection.BuildServiceProvider();
        var testDbContext = ServiceProvider.GetRequiredService<TDbContext>();
        testDbContext.Database.EnsureCreated();
        DbContext = testDbContext;
    }

    protected TDbContext DbContext { get; }

    protected ServiceProvider ServiceProvider { get; }

    protected virtual void ConfigureServices(IServiceCollection services)
    { }

    protected IUnitOfWork CreateUnitOfWork()
    {
        return ServiceProvider.GetRequiredService<IUnitOfWork>();
    }

    protected IReadRepositoryAsync<TEntity, TId> CreateReadRepositoryAsync<TEntity, TId>()
        where TEntity : class, IHasId<TId>
    {
        return ServiceProvider.GetRequiredService<IReadRepositoryAsync<TEntity, TId>>();
    }

    protected IReadRepository<TEntity, TId> CreateReadRepository<TEntity, TId>()
        where TEntity : class, IHasId<TId>
    {
        return ServiceProvider.GetRequiredService<IReadRepository<TEntity, TId>>();
    }

    protected IReadWriteRepositoryAsync<TEntity, TId> CreateRepositoryAsync<TEntity, TId>()
        where TEntity : class, IHasId<TId>
    {
        return ServiceProvider.GetRequiredService<IReadWriteRepositoryAsync<TEntity, TId>>();
    }

    protected IReadWriteRepository<TEntity, TId> CreateRepository<TEntity, TId>()
        where TEntity : class, IHasId<TId>
    {
        return ServiceProvider.GetRequiredService<IReadWriteRepository<TEntity, TId>>();
    }

    public void Dispose()
    {
        Dispose(true);
        GC.SuppressFinalize(this);
    }

    protected virtual void Dispose(bool disposing)
    {
        ServiceProvider.Dispose();
    }
}