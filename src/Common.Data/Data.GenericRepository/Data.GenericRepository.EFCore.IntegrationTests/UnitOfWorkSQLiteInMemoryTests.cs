using Microsoft.Extensions.DependencyInjection;
using Ploch.Common.Data.GenericRepository.EFCore.IntegrationTesting;
using Ploch.Common.Data.GenericRepository.EFCore.IntegrationTests.Data;
using Ploch.Common.Data.GenericRepository.EFCore.IntegrationTests.Model;

namespace Ploch.Common.Data.GenericRepository.EFCore.IntegrationTests;

public class UnitOfWorkSQLiteInMemoryTests : IDisposable
{
    private readonly ServiceProvider _serviceProvider =
        ServiceProviderBuilder.BuildServiceProviderWithInMemorySqlite<TestDbContext, TestUnitOfWork>(typeof(TestRepository<,>));

    public void Dispose()
    {
        _serviceProvider.Dispose();
        GC.SuppressFinalize(this);
    }

    [Fact]
    public async Task RepositoryAsync_and_UnitOfWorkAsync_add_and_query_by_id_should_create_entities_and_find_them()
    {
        using var scope = _serviceProvider.CreateScope();

        using var unitOfWork = scope.ServiceProvider.GetRequiredService<IUnitOfWork>();

        var (blog, blogPost1, blogPost2) = await RepositoryHelper.AddAsyncTestBlogEntitiesAsync(unitOfWork.Repository<Blog, int>());

        var userIdeas = await RepositoryHelper.AddAsyncTestUserIdeasEntitiesAsync(unitOfWork.Repository<UserIdea, int>());

        await unitOfWork.CommitAsync();

        var unitOfWork2 = scope.ServiceProvider.GetRequiredService<IUnitOfWork>();

        var blogRepository = scope.ServiceProvider.GetRequiredService<IRepositoryAsync<Blog, int>>();

        var actualBlog = await blogRepository.GetByIdAsync(blog.Id);
        actualBlog.Should().BeEquivalentTo(blog);

        var actualBlogPost1 = await unitOfWork2.Repository<BlogPost, int>().GetByIdAsync(blogPost1.Id);
        actualBlogPost1.Should().BeEquivalentTo(blogPost1);

        var actualBlogPost2 = await unitOfWork2.Repository<BlogPost, int>().GetByIdAsync(blogPost2.Id);
        actualBlogPost2.Should().BeEquivalentTo(blogPost2);

        var testUnitOfWork = scope.ServiceProvider.GetRequiredService<IUnitOfWork>();

        var actualIdeas = await testUnitOfWork.Repository<UserIdea, int>().GetAllAsync();

        actualIdeas.Should().HaveCount(2);

        actualIdeas.Should().BeEquivalentTo(userIdeas);
    }

    [Fact]
    public async Task UpdateAsync_entity()
    {
        using var scope = _serviceProvider.CreateScope();

        using var unitOfWork = scope.ServiceProvider.GetRequiredService<IUnitOfWork>();

        var (blog, blogPost1, blogPost2) = await RepositoryHelper.AddAsyncTestBlogEntitiesAsync(unitOfWork.Repository<Blog, int>());

        await unitOfWork.CommitAsync();

        var blogUpdated = new Blog { Id = blog.Id, Name = "Updated Blog" };

        await unitOfWork.Repository<Blog, int>().UpdateAsync(blogUpdated);

        var blogRepository = scope.ServiceProvider.GetRequiredService<IRepositoryAsync<Blog, int>>();

        var actualBlog = await blogRepository.GetByIdAsync(blog.Id);
        blog.Name = "Updated Blog";
        actualBlog.Should().BeEquivalentTo(blog);
    }

    [Fact]
    public async Task AddAsync_entity()
    {
        using var scope = _serviceProvider.CreateScope();

        using var unitOfWork = scope.ServiceProvider.GetRequiredService<IUnitOfWork>();

        var (blog, blogPost1, blogPost2) = await RepositoryHelper.AddAsyncTestBlogEntitiesAsync(unitOfWork.Repository<Blog, int>());

        await unitOfWork.CommitAsync();

        var blogRepository = scope.ServiceProvider.GetRequiredService<IRepositoryAsync<Blog, int>>();

        var actualBlog = await blogRepository.GetByIdAsync(blog.Id);
        actualBlog.Should().BeEquivalentTo(blog);
    }

    [Fact]
    public async Task DeleteAsync_entity()
    {
        using var scope = _serviceProvider.CreateScope();

        using var unitOfWork = scope.ServiceProvider.GetRequiredService<IUnitOfWork>();

        var (blog, blogPost1, blogPost2) = await RepositoryHelper.AddAsyncTestBlogEntitiesAsync(unitOfWork.Repository<Blog, int>());

        await unitOfWork.CommitAsync();

        var actualBlog = await unitOfWork.Repository<Blog, int>().GetByIdAsync(blog.Id);
        await unitOfWork.Repository<Blog, int>().DeleteAsync(actualBlog);
        await unitOfWork.CommitAsync();

        var deletedBlog = await unitOfWork.Repository<Blog, int>().GetByIdAsync(blog.Id);
        deletedBlog.Should().BeNull();

        var deletedBloPost1 = await unitOfWork.Repository<BlogPost, int>().GetByIdAsync(blogPost1.Id);
        deletedBloPost1.Should().BeNull();

        var deletedBloPost2 = await unitOfWork.Repository<BlogPost, int>().GetByIdAsync(blogPost2.Id);
        deletedBloPost2.Should().BeNull();
    }

    [Fact]
    public async Task Delete_entity()
    {
        using var scope = _serviceProvider.CreateScope();

        using var unitOfWork = scope.ServiceProvider.GetRequiredService<IUnitOfWork>();

        var repositoryAsync = unitOfWork.Repository<Blog, int>();

        var (blog, blogPost1, blogPost2) = await RepositoryHelper.AddAsyncTestBlogEntitiesAsync(repositoryAsync);

        await unitOfWork.CommitAsync();

        var actualBlog = await repositoryAsync.GetByIdAsync(blog.Id);
        await repositoryAsync.DeleteAsync(actualBlog);
        await unitOfWork.CommitAsync();

        var deletedBlog = await repositoryAsync.GetByIdAsync(blog.Id);
        deletedBlog.Should().BeNull();

        var deletedBloPost1 = await unitOfWork.Repository<BlogPost, int>().GetByIdAsync(blogPost1.Id);
        deletedBloPost1.Should().BeNull();

        var deletedBloPost2 = await unitOfWork.Repository<BlogPost, int>().GetByIdAsync(blogPost2.Id);
        deletedBloPost2.Should().BeNull();
    }
}