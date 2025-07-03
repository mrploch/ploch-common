using System.Collections.ObjectModel;
using AutoFixture.Xunit2;
using FluentAssertions;
using Objectivity.AutoFixture.XUnit2.AutoMoq.Attributes;
using Ploch.Common.Collections;
using Xunit;

namespace Ploch.Common.Tests.Collections;

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
        var act = static () =>
                  {
                      List<KeyValuePair<string, string>>? list = null;
#pragma warning disable CS8604 Possible null reference argument. - this is the point of the test
#pragma warning disable CS8620 Argument cannot be used for parameter due to differences in the nullability of reference types. - this is the point of the test
                      list.Add("a", "b");
#pragma warning restore CS8620
#pragma warning restore CS8604
                  };

        act.Should().Throw<ArgumentNullException>().WithParameterName("collection");
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
        target.Should().Contain([ "itme1", "item2" ], "item3", "item4");

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

    [Fact]
    public void AddMany_should_throw_when_adding_duplicate_items_with_Throw_handling()
    {
        // Arrange
        var collection = ItemsCollection(1, 2);

        // Act
        var act = () => collection.AddMany(DuplicateHandling.Throw, ItemsArray(3, 1, 4));

        // Assert
        act.Should().Throw<ArgumentException>().Where(e => e.Message.Contains("item1") && e.ParamName == "items");

        // Verify only items before the duplicate were added
        collection.Should().HaveCount(3);
        collection.Should().Contain(ItemsCollection(1, 2, 3));
    }

    [Fact]
    public void AddMany_should_skip_duplicate_items_when_IgnoreHandling_is_specified()
    {
        // Arrange
        var collection = ItemsCollection(1, 2);

        // Act
        var result = collection.AddMany(DuplicateHandling.Ignore, "item3", "item1", "item4");

        // Assert
        collection.Should().HaveCount(4);
        collection.Should().Contain(ItemsCollection(1, 2, 3, 4));
        result.Should().BeSameAs(collection);
    }

    [Fact]
    public void AddMany_should_replace_duplicate_items_when_Overwrite_handling_is_specified()
    {
        // Arrange
        var collection = ItemsList(1, 2);
        var initialItem = collection[0]; // Reference to the first item

        // Act
        var result = collection.AddMany(DuplicateHandling.Overwrite, ItemsArray(3, 1, 4)); /*"item3", "item1", "item4"*/

        // Assert
        collection.Should().HaveCount(4);
        collection.Should().Contain(ItemsCollection(1, 2, 3, 4)); /*[ "item1", "item2", "item3", "item4" ]*/
        result.Should().BeSameAs(collection);

        // Verify the first occurrence of "item1" was replaced (removed and re-added)
        // This is important for collections where removal and re-addition has side effects
        collection[0].Should().NotBeSameAs(initialItem);
    }

    [Fact]
    public void AddMany_should_handle_empty_array_of_items()
    {
        // Arrange
        var collection = ItemsCollection(1, 2);
        var initialCount = collection.Count;

        // Act
        var result = collection.AddMany(DuplicateHandling.Throw, Array.Empty<string>());

        // Assert
        collection.Should().HaveCount(initialCount);
        collection.Should().Contain(ItemsCollection(1, 2));
        result.Should().BeSameAs(collection);
    }

    [Fact]
    public void AddMany_with_params_should_throw_when_collection_is_null()
    {
        // Arrange
        ICollection<string>? collection = null;

        // Act
        var act = () =>
                  {
#pragma warning disable CS8604 // Possible null reference argument - this is the point of the test
#pragma warning disable CS8631 // The type cannot be used as type parameter in the generic type or method. Nullability of type argument doesn't match constraint type.
                      collection.AddMany(DuplicateHandling.Throw, "item1", "item2");
#pragma warning restore CS8631 // The type cannot be used as type parameter in the generic type or method. Nullability of type argument doesn't match constraint type.
#pragma warning restore CS8604
                  };

        // Assert
        act.Should().Throw<ArgumentNullException>().WithParameterName("collection");
    }

    [Fact]
    public void AddMany_should_handle_null_items_within_array()
    {
        // Arrange
        var collection = ItemsCollection(1, 2);
        var initialCount = collection.Count;
        string? nullItem = null;

        // Act
#pragma warning disable CS8631 // The type cannot be used as type parameter in the generic type or method. Nullability of type argument doesn't match constraint type.
        var result = collection.AddMany(DuplicateHandling.Throw, "item3", nullItem, "item4");
#pragma warning restore CS8631 // The type cannot be used as type parameter in the generic type or method. Nullability of type argument doesn't match constraint type.

        // Assert
        collection.Should().HaveCount(initialCount + 3);
        collection.Should().Contain(new[] { "item1", "item2", "item3", null, "item4" });
        result.Should().BeSameAs(collection);
    }

    private static IList<string> ItemsList(params int[] itemNumbers) => itemNumbers.Select(n => $"item{n}").ToList();

    private static ICollection<string> ItemsCollection(params int[] itemNumbers) => ItemsList(itemNumbers);

    private static string[] ItemsArray(params int[] itemNumbers) => ItemsList(itemNumbers).ToArray();
}
