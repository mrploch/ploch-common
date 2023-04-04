using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics.CodeAnalysis;
using AutoFixture.Xunit2;
using FluentAssertions;
using Objectivity.AutoFixture.XUnit2.AutoMoq.Attributes;
using Ploch.Common.Collections;
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
            var list = new Collection<KeyValuePair<string, int>>();

            list.Add("test1", 1);
            list.Add("test2", 2).Add("test3", 3);
            list.Should().HaveCount(3);

            list.Should().Contain(new List<KeyValuePair<string, int>> { new("test1", 1), new("test2", 2), new("test3", 3) });
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

        [Theory]
        [AutoMockData]
        public void AddMany_should_extend_collection_with_items(string[] items)
        {
            var target = new Collection<string> { "itme1", "item2" };

            target.AddMany(items);
            target.Should().HaveCount(items.Length + 2);
            var expected = new List<string> { "itme1", "item2" };
            expected.AddRange(items);
            target.Should().Contain(expected);
        }

        [Fact]
        public void AddMany_should_extend_collection_with__coll_items()
        {
            var target = new Collection<string> { "itme1", "item2" };
            var items = new Collection<string> { "item3", "item4" };

            target.AddMany(items);
            target.Should().HaveCount(4);
            target.Should().Contain(new[] { "itme1", "item2" }, "item3", "item4");
        }

        [Theory]
        [AutoData]
        public void AddIfNotNull_adds_item_to_dictionary_if_value_is_not_null(Dictionary<string, string> dict)
        {
            var notNullkey = "notNullKey";
            var nullKey = "nullKey";
            var notNullValue = "notNullValue";
            dict.AddIfNotNull(notNullkey, notNullValue).AddIfNotNull(nullKey, null);

            dict.Should().Contain(notNullkey, notNullValue);
            dict.Should().NotContainKey(nullKey);
        }
    }
}