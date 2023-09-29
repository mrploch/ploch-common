using System.Data.Common;
using Microsoft.Data.Sqlite;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;

namespace Ploch.Common.Data.GenericRepository.EFCore.IntegrationTesting;

public static class ServiceProviderBuilder
{
    public static ServiceProvider BuildServiceProviderWithInMemorySqlite<TDbContext, TUnitOfWork>(Type repositoryType,
                                                                                                  Action<IServiceCollection>? serviceCollectionAction = null)
        where TDbContext : DbContext where TUnitOfWork : class, IUnitOfWork
    {
        var connection = new SqliteConnection("Filename=:memory:");
        connection.Open();

        var serviceProvider = BuildServiceProvider<TDbContext, TUnitOfWork>(repositoryType,
                                                                            builder => builder.UseSqlite(connection),
                                                                            collection =>
                                                                            {
                                                                                collection.AddSingleton<DbConnection>(connection);
                                                                                serviceCollectionAction?.Invoke(collection);
                                                                            });

        serviceProvider.GetRequiredService<TDbContext>().Database.EnsureCreated();

        return serviceProvider;
    }

    public static ServiceProvider BuildServiceProvider<TDbContext, TUnitOfWork>(Type repositoryType,
                                                                                Action<DbContextOptionsBuilder>? optionsAction = null,
                                                                                Action<IServiceCollection>? serviceCollectionAction = null)
        where TDbContext : DbContext where TUnitOfWork : class, IUnitOfWork
    {
        // Guard.Argument(repositoryType)
        //      .NotNull()
        //      .Require(repositoryType.IsAssignableTo(typeof(IRepositoryAsync<,>)), x => $"{x} must be assignable to {typeof(IRepositoryAsync<,>)}");
        var serviceCollection = new ServiceCollection();
        serviceCollection.AddDbContext<TDbContext>(optionsAction);
        serviceCollection.AddTransient(typeof(IRepositoryAsync<,>), repositoryType);
        serviceCollection.AddTransient<IUnitOfWork, TUnitOfWork>();

        serviceCollectionAction?.Invoke(serviceCollection);

        return serviceCollection.BuildServiceProvider();
    }
}