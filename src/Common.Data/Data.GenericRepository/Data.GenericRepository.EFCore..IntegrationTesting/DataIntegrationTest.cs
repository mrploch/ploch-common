using Microsoft.Data.Sqlite;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;

namespace Ploch.Common.Data.GenericRepository.EFCore.IntegrationTesting;

public abstract class DataIntegrationTest<TDbContext> : IDisposable where TDbContext : DbContext
{
    protected DataIntegrationTest()
    {
        var connection = new SqliteConnection("Filename=:memory:");
        connection.Open();
        var serviceCollection = new ServiceCollection();

        ConfigureServices(serviceCollection);

        serviceCollection.AddDbContext<TDbContext>(builder => builder.UseSqlite(connection));
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

    public virtual void Dispose()
    {
        ServiceProvider.Dispose();
        GC.SuppressFinalize(this);
    }
}