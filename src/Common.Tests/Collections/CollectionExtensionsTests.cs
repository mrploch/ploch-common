using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using AutoFixture.Xunit2;
using FluentAssertions;
using Objectivity.AutoFixture.XUnit2.AutoMoq.Attributes;
using Ploch.Common.Collections;
using Xunit;

namespace Ploch.Common.Tests.Collections
{
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

        [Fact]
        public void KeyValuePairThrowsOnNullCollection()
        {
            Assert.Throws<ArgumentNullException>(static () =>
                                                 {
                                                     List<KeyValuePair<string, string>>? list = null;
#pragma warning disable CS8604 Possible null reference argument. - this is the point of the test
#pragma warning disable CS8620 Argument cannot be used for parameter due to differences in the nullability of reference types. - this is the point of the test
                                                     list.Add("a", "b");
#pragma warning restore CS8620
#pragma warning restore CS8604
                                                 });
        }

        [Theory]
        [AutoMockData]
        public void AddMany_should_extend_collection_with_items(string[] items)
        {
            var target = new Collection<string> { "itme1", "item2" };

            var collection = target.AddMany(items);
            target.Should().HaveCount(items.Length + 2);
            var expected = new List<string> { "itme1", "item2" };
            expected.AddRange(items);
            target.Should().Contain(expected);

            collection.Should().BeSameAs(target);
        }

        [Fact]
        public void AddMany_should_extend_collection_with__coll_items()
        {
            var target = new Collection<string> { "itme1", "item2" };
            var items = new Collection<string> { "item3", "item4" };

            var collection = target.AddMany(items);
            target.Should().HaveCount(4);
            target.Should().Contain(new[] { "itme1", "item2" }, "item3", "item4");

            collection.Should().BeSameAs(target);
        }

        [Theory]
        [AutoData]
        public void AddIfNotNull_adds_item_to_dictionary_if_value_is_not_null(Dictionary<string, string> dict)
        {
            var notNullKey = "notNullKey";
            var nullKey = "nullKey";
            var notNullValue = "notNullValue";
            var collection = dict!.AddIfNotNull(notNullKey, notNullValue).AddIfNotNull(nullKey, null);

            dict.Should().Contain(notNullKey, notNullValue);
            dict.Should().NotContainKey(nullKey);
            collection.Should().BeSameAs(dict!);
        }
    }
}