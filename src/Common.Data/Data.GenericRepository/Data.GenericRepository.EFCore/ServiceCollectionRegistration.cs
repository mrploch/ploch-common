using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;

namespace Ploch.Common.Data.GenericRepository.EFCore;

public static class ServiceCollectionRegistration
{
    public static IServiceCollection AddRepositories<TDbContext>(this IServiceCollection serviceCollection) where TDbContext : DbContext
    {
        serviceCollection.AddTransient<DbContext>(provider => provider.GetRequiredService<TDbContext>());
        serviceCollection.AddScoped(typeof(IReadRepository<,>), typeof(ReadRepository<,>));
        serviceCollection.AddScoped(typeof(IReadRepositoryAsync<,>), typeof(ReadRepositoryAsync<,>));
        serviceCollection.AddScoped(typeof(IRepository<,>), typeof(Repository<,>));
        serviceCollection.AddScoped(typeof(IRepositoryAsync<,>), typeof(RepositoryAsync<,>));

        serviceCollection.AddScoped<IUnitOfWork, UnitOfWork>();

        return serviceCollection;
    }
}