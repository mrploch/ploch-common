using Microsoft.Extensions.DependencyInjection;
using Ploch.Common.Data.GenericRepository.EFCore.IntegrationTests.Model;
using Ploch.Common.Data.Repositories.Interfaces;

namespace Ploch.Common.Data.GenericRepository.EFCore.IntegrationTests;

public class UnitOfWorkSQLiteInMemoryTests : IDisposable
{
    private readonly ServiceProvider _serviceProvider = ServiceProviderBuilder.BuildServiceProviderWithInMemorySqlite();

    public void Dispose()
    {
        _serviceProvider.Dispose();
        GC.SuppressFinalize(this);
    }

    [Fact]
    public async Task RepositoryAsync_and_UnitOfWorkAsync_add_and_query_by_id_should_create_entities_and_find_them()
    {
        var unitOfWork = _serviceProvider.GetRequiredService<IUnitOfWork>();

        var (blog, blogPost1, blogPost2) = await AddTestBlogEntitiesAsync(unitOfWork);

        var userIdeas = await AddTestUserIdeasEntitiesAsync(unitOfWork);

        await unitOfWork.CommitAsync();

        unitOfWork.Dispose();

        unitOfWork = _serviceProvider.GetRequiredService<IUnitOfWork>();

        var blogRepository = _serviceProvider.GetRequiredService<IRepositoryAsync<Blog, int>>();

        var actualBlog = await blogRepository.GetByIdAsync(blog.Id);
        actualBlog.Should().BeEquivalentTo(blog);

        var actualBlogPost1 = await unitOfWork.Repository<BlogPost, int>().GetByIdAsync(blogPost1.Id);
        actualBlogPost1.Should().BeEquivalentTo(blogPost1);

        var actualBlogPost2 = await unitOfWork.Repository<BlogPost, int>().GetByIdAsync(blogPost2.Id);
        actualBlogPost2.Should().BeEquivalentTo(blogPost2);

        var testUnitOfWork = _serviceProvider.GetRequiredService<IUnitOfWork>();

        var actualIdeas = await testUnitOfWork.Repository<UserIdea, int>().GetAllAsync();

        actualIdeas.Should().HaveCount(2);

        actualIdeas.Should().BeEquivalentTo(userIdeas);
    }

    private static async Task<(Blog, BlogPost, BlogPost)> AddTestBlogEntitiesAsync(IUnitOfWork unitOfWork)
    {
        var blogRepository = unitOfWork.Repository<Blog, int>();

        var (blog, blogPost1, blogPost2) = EntitiesBuilder.BuildBlogEntity();

        await blogRepository.AddAsync(blog);

        return (blog, blogPost1, blogPost2);
    }

    private static async Task<IEnumerable<UserIdea>> AddTestUserIdeasEntitiesAsync(IUnitOfWork unitOfWork)
    {
        var userIdeasRepository = unitOfWork.Repository<UserIdea, int>();

        var (userIdea1, userIdea2) = EntitiesBuilder.BuildUserIdeaEntities();

        await userIdeasRepository.AddAsync(userIdea1);
        await userIdeasRepository.AddAsync(userIdea2);

        return new[] { userIdea1, userIdea2 };
    }
}