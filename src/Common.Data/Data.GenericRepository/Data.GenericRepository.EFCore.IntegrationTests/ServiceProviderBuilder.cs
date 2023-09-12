using System.Data.Common;
using Microsoft.Data.Sqlite;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Ploch.Common.Data.GenericRepository.EFCore.IntegrationTests.Data;
using Ploch.Common.Data.Repositories.Interfaces;

namespace Ploch.Common.Data.GenericRepository.EFCore.IntegrationTests;

public static class ServiceProviderBuilder
{
    public static ServiceProvider BuildServiceProviderWithInMemorySqlite(Action<IServiceCollection>? serviceCollectionAction = null)
    {
        var connection = new SqliteConnection("Filename=:memory:");
        connection.Open();

        var serviceProvider = BuildServiceProvider(builder => builder.UseSqlite(connection),
                                                   collection =>
                                                   {
                                                       collection.AddSingleton<DbConnection>(connection);
                                                       serviceCollectionAction?.Invoke(collection);
                                                   });

        serviceProvider.GetRequiredService<TestDbContext>().Database.EnsureCreated();

        return serviceProvider;
    }

    public static ServiceProvider BuildServiceProvider(Action<DbContextOptionsBuilder>? optionsAction = null, Action<IServiceCollection>? serviceCollectionAction = null)
    {
        var serviceCollection = new ServiceCollection();
        serviceCollection.AddDbContext<TestDbContext>(optionsAction);
        serviceCollection.AddTransient(typeof(IRepositoryAsync<,>), typeof(TestRepository<,>));
        serviceCollection.AddTransient<IUnitOfWork, TestUnitOfWork>();

        serviceCollectionAction?.Invoke(serviceCollection);

        return serviceCollection.BuildServiceProvider();
    }
}