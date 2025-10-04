using Ploch.Common.Collections;

namespace Ploch.Common.Tests.Collections;

public class DictionaryExtensionsTests
{
    [Fact]
    public void AddMany_should_add_entries_to_dictionary_and_return_it()
    {
        var dictionary = new Dictionary<string, string> { { "key0", "value0" } };

        KeyValuePair<string, string>[] entries = [new("key1", "value1"), new("key2", "value2"), new("key3", "value3")];

        var result = dictionary.AddMany(entries);

        result.Should().BeSameAs(dictionary);
        result.Should().HaveCount(4);
        result.Should()
              .Contain(new KeyValuePair<string, string>("key0", "value0"),
                       new KeyValuePair<string, string>("key1", "value1"),
                       new KeyValuePair<string, string>("key2", "value2"),
                       new KeyValuePair<string, string>("key3", "value3"));
    }

    [Fact]
    public void AddMany_should_throw_ArgumentNullException_when_dictionary_is_null()
    {
        // Arrange
        Dictionary<string, string>? dictionary = null;
        KeyValuePair<string, string>[] entries = [new("key1", "value1")];

        // Act & Assert
        var action = () => dictionary!.AddMany(entries);

        action.Should().Throw<ArgumentNullException>().WithParameterName("dictionary");
    }

    [Fact]
    public void AddMany_should_throw_ArgumentNullException_when_items_collection_is_null()
    {
        // Arrange
        var dictionary = new Dictionary<string, string> { { "key0", "value0" } };
        IEnumerable<KeyValuePair<string, string>>? items = null;

        // Act & Assert
        var action = () => dictionary.AddMany(items!);

        action.Should().Throw<ArgumentNullException>().WithParameterName("items");
    }

    [Fact]
    public void AddMany_should_throw_ArgumentException_when_duplicate_key_exists_and_DuplicateHandling_is_Throw()
    {
        // Arrange
        var dictionary = new Dictionary<string, string> { { "key1", "value1" } };
        KeyValuePair<string, string>[] entries = [new("key1", "newValue"), new("key2", "value2")];

        // Act & Assert
        var action = () => dictionary.AddMany(entries);

        action.Should().Throw<ArgumentException>().WithMessage("*key1*already exists*").WithParameterName("items");
    }

    [Fact]
    public void AddMany_should_overwrite_existing_values_when_duplicate_key_exists_and_DuplicateHandling_is_Overwrite()
    {
        // Arrange
        var dictionary = new Dictionary<string, string>
                             {
                                 { "key1", "originalValue" },
                                 { "key2", "value2" }
                             };
        KeyValuePair<string, string>[] entries = [new("key1", "newValue"), new("key3", "value3")];

        // Act
        var result = dictionary.AddMany(entries, DuplicateHandling.Overwrite);

        // Assert
        result.Should().BeSameAs(dictionary);
        result.Should().HaveCount(3);
        result.Should()
              .Contain(new KeyValuePair<string, string>("key1", "newValue"),
                       new KeyValuePair<string, string>("key2", "value2"),
                       new KeyValuePair<string, string>("key3", "value3"));
    }

    [Fact]
    public void AddMany_should_ignore_existing_entries_when_duplicate_key_exists_and_DuplicateHandling_is_Ignore()
    {
        // Arrange
        var dictionary = new Dictionary<string, string>
                             {
                                 { "key1", "originalValue" },
                                 { "key2", "value2" }
                             };
        KeyValuePair<string, string>[] entries = [new("key1", "newValue"), new("key3", "value3")];

        // Act
        var result = dictionary.AddMany(entries, DuplicateHandling.Ignore);

        // Assert
        result.Should().BeSameAs(dictionary);
        result.Should().HaveCount(3);
        result.Should()
              .Contain(new KeyValuePair<string, string>("key1", "originalValue"),
                       new KeyValuePair<string, string>("key2", "value2"),
                       new KeyValuePair<string, string>("key3", "value3"));
    }

    [Fact]
    public void AddMany_should_throw_ArgumentOutOfRangeException_when_invalid_DuplicateHandling_value_is_provided()
    {
        // Arrange
        var dictionary = new Dictionary<string, string> { { "key1", "value1" } };
        KeyValuePair<string, string>[] entries = [new("key2", "value2")];

        // Using an invalid enum value by casting an int outside the defined enum values
        var invalidDuplicateHandling = (DuplicateHandling)999;

        // Act & Assert
        var action = () => dictionary.AddMany(entries, invalidDuplicateHandling);

        action.Should().Throw<ArgumentOutOfRangeException>().WithParameterName("duplicateHandling").And.ActualValue.Should().Be(invalidDuplicateHandling);
    }

    [Fact]
    public void AddMany_should_handle_empty_items_collection_without_modifying_dictionary()
    {
        // Arrange
        var dictionary = new Dictionary<string, string>
                             {
                                 { "key1", "value1" },
                                 { "key2", "value2" }
                             };
        var emptyItems = Array.Empty<KeyValuePair<string, string>>();

        // Act
        var result = dictionary.AddMany(emptyItems);

        // Assert
        result.Should().BeSameAs(dictionary);
        result.Should().HaveCount(2);
        result.Should().Contain(new KeyValuePair<string, string>("key1", "value1"), new KeyValuePair<string, string>("key2", "value2"));
    }

    [Fact]
    public void AddMany_should_work_with_non_string_key_and_value_types()
    {
        // Arrange
        var dictionary = new Dictionary<int, decimal> { { 1, 10.5m } };
        KeyValuePair<int, decimal>[] entries = [new(2, 20.75m), new(3, 30.25m), new(4, 40.0m)];

        // Act
        var result = dictionary.AddMany(entries);

        // Assert
        result.Should().BeSameAs(dictionary);
        result.Should().HaveCount(4);
        result.Should()
              .Contain(new KeyValuePair<int, decimal>(1, 10.5m),
                       new KeyValuePair<int, decimal>(2, 20.75m),
                       new KeyValuePair<int, decimal>(3, 30.25m),
                       new KeyValuePair<int, decimal>(4, 40.0m));
    }

    [Fact]
    public void AddMany_should_maintain_original_dictionary_content_when_adding_no_new_items()
    {
        // Arrange
        var dictionary = new Dictionary<string, string>
                             {
                                 { "key1", "value1" },
                                 { "key2", "value2" },
                                 { "key3", "value3" }
                             };
        var originalDictionary = new Dictionary<string, string>(dictionary);
        var emptyItems = new List<KeyValuePair<string, string>>();

        // Act
        var result = dictionary.AddMany(emptyItems);

        // Assert
        result.Should().BeSameAs(dictionary);
        result.Should().HaveCount(originalDictionary.Count);
        result.Should().BeEquivalentTo(originalDictionary);

        // Verify each key-value pair is unchanged
        foreach (var pair in originalDictionary)
        {
            result[pair.Key].Should().Be(pair.Value);
        }
    }

    [Fact]
    public void AddMany_should_work_with_different_dictionary_implementations()
    {
        // Arrange
        // SortedDictionary is a different implementation of IDictionary<TKey,TValue>
        var dictionary = new SortedDictionary<string, int>
                             {
                                 { "a", 1 },
                                 { "c", 3 }
                             };
        KeyValuePair<string, int>[] entries = [new("b", 2), new("d", 4)];

        // Act
        var result = dictionary.AddMany(entries);

        // Assert
        result.Should().BeSameAs(dictionary);
        result.Should().HaveCount(4);
        result.Should()
              .Contain(new KeyValuePair<string, int>("a", 1),
                       new KeyValuePair<string, int>("b", 2),
                       new KeyValuePair<string, int>("c", 3),
                       new KeyValuePair<string, int>("d", 4));

        // Also verify that the SortedDictionary's natural ordering is maintained
        result.Keys.Should().BeInAscendingOrder();
    }
}
