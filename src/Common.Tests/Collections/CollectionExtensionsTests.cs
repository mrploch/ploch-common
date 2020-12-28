using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics.CodeAnalysis;
using FluentAssertions;
using Ploch.Common.Collections;
using Ploch.TestingSupport.Xunit.AutoFixture;
using Xunit;


namespace Ploch.Common.Tests.Collections
{
    [SuppressMessage("ReSharper", "ExceptionNotDocumentedOptional")]
    [SuppressMessage("ReSharper", "ExceptionNotDocumentedOptional")]
    public class CollectionExtensionsTests
    {
        [Fact]
        public void KeyValuePairAdd()
        {
            var list = new List<KeyValuePair<string, int>>();

            list.Add("test1", 1);
            list.Add("test2", 2).Add("test3", 3);
            list.Should().HaveCount(3);

            list.Should()
                .Contain(new List<KeyValuePair<string, int>>
                {
                    new KeyValuePair<string, int>("test1", 1), new KeyValuePair<string, int>("test2", 2), new KeyValuePair<string, int>("test3", 3)
                });
        }

        [SuppressMessage("ReSharper", "ExpressionIsAlwaysNull")]
        [Fact]
        public void KeyValuePairThrowsOnNullCollection()
        {
            Assert.Throws<ArgumentNullException>(() =>
            {
                List<KeyValuePair<string, string>> list = null;
                list.Add("a", "b");
            });
        }



        [Theory, AutoDataMoq]
        public void AddMany_should_extend_collection_with_items(string[] items)
        {
            var target = new Collection<string>() {"itme1", "item2"};
            
            target.AddMany(items);
            target.Should().HaveCount(items.Length + 2);
            target.Should().Contain(new[] {"itme1", "item2"}, items);
        }

        [Fact]
        public void AddMany_should_extend_collection_with__coll_items()
        {
            var target = new Collection<string>() { "itme1", "item2" };
            var items = new Collection<string>() { "item3", "item4"};

            target.AddMany(items);
            target.Should().HaveCount(4);
            target.Should().Contain(new[] { "itme1", "item2" }, "item3", "item4");
        }

    }
}