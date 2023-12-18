using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Ploch.Common.Data.GenericRepository.EFCore.IntegrationTesting;
using Ploch.Common.Data.GenericRepository.EFCore.IntegrationTests.Data;
using Ploch.Common.Data.GenericRepository.EFCore.IntegrationTests.Model;

namespace Ploch.Common.Data.GenericRepository.EFCore.IntegrationTests;

public class ServiceCollectionRegistrationsTests
{
    [Fact]
    public void AddRepositories_should_register_repository_types_mapping_them_to_concrete_implementation()
    {
        var serviceCollection = new ServiceCollection();

        RepositoryServicesRegistrationHelper.RegisterRepositoryServices<TestDbContext>(serviceCollection);

        var serviceProvider = serviceCollection.BuildServiceProvider();

        serviceProvider.GetRequiredService<IReadRepository<Blog, int>>().Should().BeOfType<ReadRepository<Blog, int>>();
        serviceProvider.GetRequiredService<IReadRepositoryAsync<Blog>>().Should().BeOfType<ReadRepositoryAsync<Blog>>();
        serviceProvider.GetRequiredService<IReadRepositoryAsync<Blog, int>>().Should().BeOfType<ReadRepositoryAsync<Blog, int>>();
        serviceProvider.GetRequiredService<IWriteRepository<Blog, int>>().Should().BeOfType<ReadWriteRepository<Blog, int>>();
        serviceProvider.GetRequiredService<IWriteRepositoryAsync<Blog, int>>().Should().BeOfType<ReadWriteRepositoryAsync<Blog, int>>();
        serviceProvider.GetRequiredService<IReadWriteRepository<Blog, int>>().Should().BeOfType<ReadWriteRepository<Blog, int>>();
        serviceProvider.GetRequiredService<IReadWriteRepositoryAsync<Blog, int>>().Should().BeOfType<ReadWriteRepositoryAsync<Blog, int>>();
    }

    [Fact]
    public void AddCustomAsyncRepository_should_register_custom_repository()
    {
        var serviceCollection = new ServiceCollection();

        serviceCollection.AddCustomAsyncRepository<ICustomBlogRepository, CustomBlogRepository, Blog, int>((collection, repositoryInterface, repositoryImpl) =>
                                                                                                               collection.AddScoped(repositoryInterface, repositoryImpl));
        serviceCollection.AddScoped<TestCommandReadRepository>();

        RepositoryServicesRegistrationHelper.RegisterRepositoryServices<TestDbContext>(serviceCollection);

        var serviceProvider = serviceCollection.BuildServiceProvider();

        serviceProvider.GetRequiredService<ICustomBlogRepository>().Should().BeOfType<CustomBlogRepository>();
        serviceProvider.GetRequiredService<TestCommandReadRepository>().Should().BeOfType<TestCommandReadRepository>();
    }

    class TestCommandReadRepository
    {
        private readonly IReadRepositoryAsync<Blog, int> _blogReadRepository;

        public TestCommandReadRepository(IReadRepositoryAsync<Blog, int> blogReadRepository)
        {
            _blogReadRepository = blogReadRepository;
        }
    }
    

    private interface ICustomBlogRepository : IReadWriteRepositoryAsync<Blog, int>
    { }

    private class CustomBlogRepository : ReadWriteRepositoryAsync<Blog, int>, ICustomBlogRepository
    {
        public CustomBlogRepository(DbContext dbContext) : base(dbContext)
        { }
    }
}