using Ploch.Common.Data.GenericRepository.EFCore.IntegrationTesting;
using Ploch.Common.Data.GenericRepository.EFCore.IntegrationTests.Data;
using Ploch.Common.Data.GenericRepository.EFCore.IntegrationTests.Model;

namespace Ploch.Common.Data.GenericRepository.EFCore.IntegrationTests;

public class ReadWriteRepositoryAsyncTests : DataIntegrationTest<TestDbContext>
{
    [Fact]
    public async Task CountAsync_should_return_entity_count()
    {
        using var unitOfWork = CreateUnitOfWork();

        var (blog, blogPost1, blogPost2) = await RepositoryHelper.AddAsyncTestBlogEntitiesAsync(unitOfWork.Repository<Blog, int>());

        await unitOfWork.CommitAsync();

        var repository = CreateReadRepositoryAsync<Blog, int>();
        var count = await repository.GetCountAsync();

        count.Should().Be(3);
    }

    [Fact]
    public async Task Count_should_return_entity_count()
    {
        using var unitOfWork = CreateUnitOfWork();

        var (blog, blogPost1, blogPost2) = await RepositoryHelper.AddAsyncTestBlogEntitiesAsync(unitOfWork.Repository<Blog, int>());

        await unitOfWork.CommitAsync();

        var repository = CreateReadRepository<Blog, int>();
        var count = repository.Count();

        count.Should().Be(3);
    }
}