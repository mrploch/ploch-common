using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;

namespace Ploch.Common.Data.GenericRepository.EFCore.IntegrationTesting;

public interface IDbContextConfigurator
{
    void Configure(DbContextOptionsBuilder optionsBuilder);
}

public record SqLiteConnectionOptions
{
    public SqLiteConnectionOptions(bool inMemory = true, string? dbFilePath = null)
    {
        if (inMemory && dbFilePath is not null)
        {
            throw new ArgumentException("Cannot specify both inMemory and dbFilePath");
        }

        if (!inMemory && dbFilePath is null)
        {
            throw new ArgumentException("Must specify either inMemory or dbFilePath");
        }

        InMemory = inMemory;
        DbFilePath = dbFilePath;
    }

    public bool InMemory { get; }

    public string? DbFilePath { get; }
}

public class SqLiteDbContextConfigurator : IDbContextConfigurator
{
    private readonly SqLiteConnectionOptions _options;

    public SqLiteDbContextConfigurator(SqLiteConnectionOptions options)
    {
        _options = options;
    }

    public void Configure(DbContextOptionsBuilder optionsBuilder)
    {
        /*
         * var connection = new SqliteConnection(connectionString);
                                                       connection.Open();
                                                         builder.UseSqlite(connection);
         */
        var dbSource = _options.InMemory ? ":memory:" : _options.DbFilePath;
        optionsBuilder.UseSqlite($"Data Source={dbSource}");
    }
}

public class DbTestServices<TDbContext> : IDisposable
    where TDbContext : DbContext
{
    private readonly Action<IServiceCollection>? _configureServices;

    public DbTestServices(IDbContextConfigurator dbContextConfigurator, Action<IServiceCollection>? configureServices = null)
    {
        ServiceProvider = BuildServiceProvider(dbContextConfigurator, configureServices);
        DbContext = BuildDbContext();
    }

    public TDbContext DbContext { get; }

    public IServiceProvider ServiceProvider { get; }

    private IServiceProvider BuildServiceProvider(IDbContextConfigurator dbContextConfigurator, Action<IServiceCollection>? configureServices = null)
    {
        var serviceCollection = new ServiceCollection();
        ConfigureDbContextServices(serviceCollection, dbContextConfigurator);
        configureServices?.Invoke(serviceCollection);

        return serviceCollection.BuildServiceProvider();
    }

    private TDbContext BuildDbContext()
    {
        var dbContext = ServiceProvider.GetRequiredService<TDbContext>();
        dbContext.Database.EnsureCreated();

        return dbContext;
    }

    protected virtual void ConfigureDbContextServices(IServiceCollection serviceCollection, IDbContextConfigurator dbContextConfigurator)
    {
        serviceCollection.AddDbContext<TDbContext>(dbContextConfigurator.Configure);
        serviceCollection.AddRepositories<TDbContext>();
    }

    /*public void Dispose()
    {
        Dispose(true);
        GC.SuppressFinalize(this);
    }

    protected virtual void Dispose(bool disposing)
    {
        if (ServiceProvider is IDisposable disposableServiceProvider)
        {
            disposableServiceProvider.Dispose();
        }
    }*/
    public void Dispose()
    {
        if (ServiceProvider is IDisposable disposableServiceProvider)
        {
            disposableServiceProvider.Dispose();
        }
    }
}

public static class SqLiteDbTestServices
{
    public static IDbContextConfigurator DbContextConfigurator => new SqLiteDbContextConfigurator(new SqLiteConnectionOptions());
}