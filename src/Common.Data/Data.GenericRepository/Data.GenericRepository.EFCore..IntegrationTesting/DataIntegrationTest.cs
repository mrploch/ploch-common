using System.Data.Common;
using Microsoft.Data.Sqlite;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;

namespace Ploch.Common.Data.GenericRepository.EFCore.IntegrationTesting;

public abstract class DataIntegrationTest<TDbContext> : IDisposable where TDbContext : DbContext
{
    protected DataIntegrationTest()
    {
        // Guard.Argument(RepositoryType, nameof(RepositoryType))
        //      .NotNull()
        //      .Require(RepositoryType.IsAssignableFrom(typeof(IRepositoryAsync<,>)), x => $"{x} must be assignable to {typeof(IRepositoryAsync<,>)}");
        ServiceProvider = BuildServiceProviderWithInMemorySqlite<TDbContext>();
    }

    protected ServiceProvider ServiceProvider { get; }

    public void Dispose()
    {
        ServiceProvider.Dispose();
        GC.SuppressFinalize(this);
    }

    protected IUnitOfWork StartUnitOfWork()
    {
        var scope = ServiceProvider.CreateScope();

        return scope.ServiceProvider.GetRequiredService<IUnitOfWork>();
    }

    protected virtual DbConnection CreateDbConnection()
    {
        return new SqliteConnection("Filename=:memory:");
    }

    private ServiceProvider BuildServiceProviderWithInMemorySqlite<TDbContext>(Action<IServiceCollection>? serviceCollectionAction = null) where TDbContext : DbContext
    {
        var connection = CreateDbConnection();
        connection.Open();

        var serviceProvider = BuildServiceProvider<TDbContext>(options => ConfigureDbContextOptions(options),
                                                               collection =>
                                                               {
                                                                   collection.AddSingleton(connection);
                                                                   serviceCollectionAction?.Invoke(collection);
                                                               });

        serviceProvider.GetRequiredService<TDbContext>().Database.EnsureCreated();

        return serviceProvider;
    }

    protected virtual DbContextOptionsBuilder ConfigureDbContextOptions(DbContextOptionsBuilder optionsBuilder)
    {
        return optionsBuilder.UseSqlite("Filename=:memory:");
    }

    private ServiceProvider BuildServiceProvider<TDbContext>(Action<DbContextOptionsBuilder>? optionsAction = null,
                                                             Action<IServiceCollection>? serviceCollectionAction = null) where TDbContext : DbContext
    {
        // Guard.Argument(repositoryType)
        //      .NotNull()
        //      .Require(repositoryType.IsAssignableTo(typeof(IRepositoryAsync<,>)), x => $"{x} must be assignable to {typeof(IRepositoryAsync<,>)}");
        var serviceCollection = new ServiceCollection();

        serviceCollection.AddDbContext<TDbContext>(builder => ConfigureDbContextOptions(builder));
        serviceCollection.AddRepositories<TDbContext>();
        // serviceCollection.AddTransient(typeof(IRepositoryAsync<,>), typeof(RepositoryAsync<,>));
        // serviceCollection.AddTransient<IUnitOfWork, UnitOfWork>();

        serviceCollectionAction?.Invoke(serviceCollection);

        return serviceCollection.BuildServiceProvider();
    }
}