using Microsoft.Extensions.DependencyInjection;
using Ploch.Common.Data.GenericRepository.EFCore.IntegrationTests.Data;

namespace Ploch.Common.Data.GenericRepository.EFCore.IntegrationTests;

public class DataContextSqLiteInMemoryTests : IDisposable
{
    private readonly TestDbContext _dbContext;
    private readonly ServiceProvider _serviceProvider = ServiceProviderBuilder.BuildServiceProviderWithInMemorySqlite();

    public DataContextSqLiteInMemoryTests()
    {
        _dbContext = _serviceProvider.GetRequiredService<TestDbContext>();
    }

    public void Dispose()
    {
        _serviceProvider.Dispose();
        GC.SuppressFinalize(this);
    }

    [Fact]
    public void DataContext_add_and_query_by_id_should_create_entities_and_find_them()
    {
        var (blog, blogPost1, blogPost2) = EntitiesBuilder.BuildBlogEntity();

        _dbContext.Blogs.Add(blog);

        var (userIdea1, userIdea2) = EntitiesBuilder.BuildUserIdeaEntities();

        _dbContext.UserIdeas.AddRange(userIdea1, userIdea2);

        _dbContext.SaveChanges();

        var actualBlog1 = _dbContext.Blogs.Find(1);
        actualBlog1.Should().BeEquivalentTo(blog);

        var actualBlogPost1 = _dbContext.BlogPosts.Find(1);
        actualBlogPost1.Should().BeEquivalentTo(blogPost1);

        var actualBlogPost2 = _dbContext.BlogPosts.First(bp => bp.Name == "Blog post 2");
        actualBlogPost2.Should().BeEquivalentTo(blogPost2);

        var actualUserIdea1 = _dbContext.UserIdeas.First(ui => ui.Id == userIdea1.Id);
        actualUserIdea1.Should().BeEquivalentTo(userIdea1);

        var actualUserIdea2 = _dbContext.UserIdeas.First(ui => ui.Id == userIdea2.Id);
        actualUserIdea2.Should().BeEquivalentTo(userIdea2);
    }
}