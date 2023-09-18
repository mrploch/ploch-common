using Ploch.Common.Data.GenericRepository.EFCore.IntegrationTests.Model;
using Ploch.Common.Data.Repositories.Interfaces;

namespace Ploch.Common.Data.GenericRepository.EFCore.IntegrationTests;

public class RepositoryHelper
{
    public static (Blog, BlogPost, BlogPost) AddTestBlogEntitiesAsync(IRepository<Blog, int> blogRepository)
    {
        var (blog, blogPost1, blogPost2) = EntitiesBuilder.BuildBlogEntity();

        blogRepository.Add(blog);

        return (blog, blogPost1, blogPost2);
    }

    public static IEnumerable<UserIdea> AddTestUserIdeasEntitiesAsync(IRepository<UserIdea, int> userIdeasRepository)
    {
        var (userIdea1, userIdea2) = EntitiesBuilder.BuildUserIdeaEntities();

        userIdeasRepository.Add(userIdea1);
        userIdeasRepository.Add(userIdea2);

        return new[] { userIdea1, userIdea2 };
    }

    public static async Task<(Blog blog, BlogPost blogPost1, BlogPost blogPost2)> AddAsyncTestBlogEntitiesAsync(IRepositoryAsync<Blog, int> blogRepository)
    {
        var (blog, blogPost1, blogPost2) = EntitiesBuilder.BuildBlogEntity();

        await blogRepository.AddAsync(blog);

        return (blog, blogPost1, blogPost2);
    }

    public static async Task<IEnumerable<UserIdea>> AddAsyncTestUserIdeasEntitiesAsync(IRepositoryAsync<UserIdea, int> userIdeasRepository)
    {
        var (userIdea1, userIdea2) = EntitiesBuilder.BuildUserIdeaEntities();

        userIdeasRepository.Add(userIdea1);
        userIdeasRepository.Add(userIdea2);

        return new[] { userIdea1, userIdea2 };
    }
}