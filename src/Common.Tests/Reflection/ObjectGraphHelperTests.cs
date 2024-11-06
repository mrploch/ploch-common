using FluentAssertions;
using Objectivity.AutoFixture.XUnit2.AutoMoq.Attributes;
using Ploch.Common.Reflection;
using Xunit;

namespace Ploch.Common.Tests.Reflection;

public class ObjectGraphHelperTests
{
    [Theory]
    [AutoMockData]
    public void ExecuteOnProperties_with_type_should_only_execute_action_on_specific_property_types(TestBlog testBlog, int expectedId, string expectedName)
    {
        testBlog.ExecuteOnProperties<IHasIdSettable<int>>(o => o.Id = expectedId);
        testBlog.ExecuteOnProperties<INamed>(o => o.Name = expectedName);

        testBlog.Id.Should().Be(expectedId);
        testBlog.Name.Should().Be(expectedName);
        foreach (var blogPost in testBlog.BlogPosts)
        {
            blogPost.Id.Should().Be(expectedId);
            blogPost.Name.Should().Be(expectedName);
            foreach (var category in blogPost.Categories)
            {
                category.Id.Should().Be(expectedId);
                category.Name.Should().Be(expectedName);
            }

            foreach (var tag in blogPost.Tags)
            {
                tag.Id.Should().Be(expectedId);
                tag.Name.Should().Be(expectedName);
            }
        }
    }

    [Theory]
    [AutoMockData]
    public void ExecuteOnProperties_should_execute_action_on_each_property(TestBlog testBlog)
    {
        testBlog.ExecuteOnProperties(o =>
                                     {
                                         if (o.GetType().IsPrimitive)
                                         {
                                             return;
                                         }

                                         if (o is IHasIdSettable<int> hasId)
                                         {
                                             hasId.Id = 1;
                                         }

                                         if (o is INamed named)
                                         {
                                             named.Name = "Test";
                                         }
                                     });

        testBlog.Id.Should().Be(1);
        testBlog.Name.Should().Be("Test");
        foreach (var blogPost in testBlog.BlogPosts)
        {
            blogPost.Id.Should().Be(1);
            blogPost.Name.Should().Be("Test");
            foreach (var category in blogPost.Categories)
            {
                category.Id.Should().Be(1);
                category.Name.Should().Be("Test");
            }

            foreach (var tag in blogPost.Tags)
            {
                tag.Id.Should().Be(1);
                tag.Name.Should().Be("Test");
            }
        }
    }

    public interface IHasId<TId>
    {
        TId Id { get; set; }
    }

    public interface IHasIdSettable<TId> : IHasId<TId>
    {
        new TId Id { get; set; }
    }

    public interface INamed
    {
        string Name { get; set; }
    }

    public class Category<TCategory, TId> : IHasIdSettable<TId>, INamed
    {
        public TId Id { get; set; } = default!;

        public required string Name { get; set; }
        public virtual ICollection<TCategory> Categories { get; set; } = new List<TCategory>();
    }

    public class Tag : IHasIdSettable<int>
    {
        public int Id { get; set; }
        public string Name { get; set; } = null!;
    }

    public class TestBlog : IHasIdSettable<int>, INamed
    {
        public int Id { get; set; }

        public required string Name { get; set; }
        public virtual ICollection<TestBlogPost> BlogPosts { get; set; } = new List<TestBlogPost>();
    }

    public class TestBlogPost : IHasIdSettable<int>, INamed
    {
        public int Id { get; set; }

        public required string Name { get; set; }
        public string? Contents { get; set; }

        public virtual ICollection<TestCategory> Categories { get; set; } = new List<TestCategory>();

        public virtual ICollection<TestTag> Tags { get; set; } = new List<TestTag>();
    }

    public class TestCategory : IHasIdSettable<int>, INamed
    {
        public int Id { get; set; }

        public string Name { get; set; } = null!;
        public virtual ICollection<TestBlogPost> BlogPosts { get; set; } = new List<TestBlogPost>();

        public TestCategory? Parent { get; set; }
    }

    public class TestTag : IHasIdSettable<int>, INamed
    {
        public int Id { get; set; }

        public string Name { get; set; } = null!;
        public virtual ICollection<TestBlogPost> BlogPosts { get; set; } = new List<TestBlogPost>();
    }
}