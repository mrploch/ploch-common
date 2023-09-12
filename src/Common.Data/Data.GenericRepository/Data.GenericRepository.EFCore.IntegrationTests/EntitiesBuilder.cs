using Ploch.Common.Data.GenericRepository.EFCore.IntegrationTests.Model;

namespace Ploch.Common.Data.GenericRepository.EFCore.IntegrationTests;

public static class EntitiesBuilder
{
    public static (Blog, BlogPost, BlogPost) BuildBlogEntity()
    {
        var category1 = new BlogPostCategory { Name = "Category 1" };
        var category1_1 = new BlogPostCategory { Name = "Category 1.1", Parent = category1 };
        var category1_2 = new BlogPostCategory { Name = "Category 1.2", Parent = category1 };
        var category1_2_1 = new BlogPostCategory { Name = "Category 1.2.1", Parent = category1_2 };

        var category2 = new BlogPostCategory { Name = "Category 2" };

        var tag1 = new BlogPostTag { Name = "Tag 1" };
        var tag2 = new BlogPostTag { Name = "Tag 2" };
        var tag3 = new BlogPostTag { Name = "Tag 3" };

        var blog = new Blog { Name = "Blog 1" };
        var blogPost1 = new BlogPost { Name = "Blog post 1", Contents = "My first blog post!" };
        blogPost1.Categories.Add(category1_2);
        blogPost1.Categories.Add(category1);
        blogPost1.Tags.Add(tag1);
        blogPost1.Tags.Add(tag3);

        var blogPost2 = new BlogPost { Name = "Blog post 2", Contents = "My second blog post!" };
        blogPost2.Categories.Add(category1_2_1);
        blogPost2.Categories.Add(category2);
        blogPost2.Tags.Add(tag2);

        blog.BlogPosts.Add(blogPost1);
        blog.BlogPosts.Add(blogPost2);

        return (blog, blogPost1, blogPost2);
    }

    public static (UserIdea, UserIdea) BuildUserIdeaEntities()
    {
        var userIdea1 = new UserIdea { Contents = "My first idea!" };
        var userIdea2 = new UserIdea { Contents = "My second idea!" };

        return (userIdea1, userIdea2);
    }
}