using AutoFixture;
using FluentAssertions;
using FluentAssertions.Collections;
using FluentAssertions.Execution;
using Objectivity.AutoFixture.XUnit2.AutoMoq.Attributes;
using Ploch.Common.Collections;
using Ploch.Common.Tests.Reflection;
using Xunit;
using Xunit.Abstractions;

namespace Ploch.Common.Tests.Collections;

public class EnumerableExtensionsTests
{
    private readonly ITestOutputHelper _output;

    public EnumerableExtensionsTests(ITestOutputHelper output)
    {
        _output = output;
    }

    [Fact]
    public void ValueIn_should_return_true_if_value_is_in_specified_set_of_values_using_default_comparer()
    {
        "val2".ValueIn("val1", "val2", "val3").Should().BeTrue();
        "test".ValueIn("val1", "val2", "val3").Should().BeFalse();
        "VAL2".ValueIn("val1", "val2", "val3").Should().BeFalse();

        10.ValueIn(1, 2, 3, 10, 20, 30);
        13.ValueIn(1, 2, 3, 10, 20, 30).Should().BeFalse();
    }

    [Fact]
    public void ValueIn_should_handle_null()
    {
        ((object)null).ValueIn(new object(), new object(), null).Should().BeTrue();
        ((string)null).ValueIn("str1", "str2").Should().BeFalse();
    }

    [Fact]
    public void ValueIn_should_return_true_if_value_is_in_specified_set_of_values_using_provided_comparer()
    {
        "VAL2".ValueIn(StringComparer.InvariantCultureIgnoreCase, "val1", "val2", "val3").Should().BeTrue();
        "test".ValueIn(StringComparer.InvariantCultureIgnoreCase, "val1", "val2", "val3").Should().BeFalse();
    }

    [Fact]
    public void None_should_return_true_if_no_item_matches_predicate()
    {
        var items = new[] { 1, 2, 3, 4, 5, 6, 7, 8 };
        items.None(i => i > 10).Should().BeTrue();
        items.None(i => i > 5).Should().BeFalse();
    }

    [Theory]
    [AutoMockData]
    public void Join_should_string_join_int_enumerable(IEnumerable<int> ints, string separator)
    {
        ints.Join(separator).Should().Be(string.Join(separator, ints));
    }

    [Theory]
    [AutoMockData]
    public void Join_should_string_join_datetime_enumerable(IEnumerable<DateTime> dateTimes, string separator)
    {
        var join = dateTimes.Join(separator);
        join.Should().Be(string.Join(separator, dateTimes));

        join = dateTimes.Join(" ", dt => dt.Year);
        join.Should().Be(string.Join(" ", dateTimes.Select(dt => dt.Year)));
    }

    [Fact]
    public void Join_should_string_join_objects_enumerable_with_different_final_separator()
    {
        var testObjects = new TestTypes.MyTestClass[] { new() { IntProp = 1 }, new() { IntProp = 2 }, new() { IntProp = 3 } };

        var join = testObjects.JoinWithFinalSeparator(", ", " and ", o => o.IntProp);

        join.Should().Be("1, 2 and 3");
    }

    [Fact]
    public void TakeRandom_should_pick_count_number_of_random_items_from_source()
    {
        var (sourceList, result) = TestTakeRandom(100, 10);

        result.Should().HaveCount(10).And.OnlyContain(s => sourceList.Contains(s)).And.NotContainInOrder(sourceList.Take(10));
        // TestTakeRandom(100, 1000);
        // TestTakeRandom(0, 100);
        // TestTakeRandom(100, 101);
    }

    [Fact]
    public void TakeRandom_should_return_source_list_size_items_if_sample_is_greater_than_source()
    {
        var (sourceList, result) = TestTakeRandom(10, 100);

        result.Should().HaveCount(10).And.OnlyContain(s => sourceList.Contains(s)).And.NotContainInOrder(sourceList.Take(10));
    }

    [Fact]
    public void TakeRandom_should_return_0_items_if_sample_size_is_0()
    {
        var (sourceList, result) = TestTakeRandom(10, 0);

        result.Should().BeEmpty();
    }

    [Fact]
    public void TakeRandom_should_return_0_items_if_source_list_size_is_0()
    {
        var (sourceList, result) = TestTakeRandom(0, 10);

        result.Should().BeEmpty();
    }

    private (List<string> list, IEnumerable<string> result) TestTakeRandom(int itemsCount, int sampleSize)
    {
        var list = new List<string>();
        for (var i = 0; i < itemsCount; i++)
        {
            list.Add($"Item {i}");
        }

        var result = list.TakeRandom(sampleSize);
        _output.WriteLine($"TakeRandom test. Sample size: {sampleSize}, items count: {itemsCount}, result count: {result.Count()}");

        return (list, result);
    }

    [Fact]
    public void Shuffle_should_randomly_shuffle_items_in_collection()
    {
        var strings = new Fixture().CreateMany<string>(20);
        var result = strings.Shuffle();

        // ReSharper disable once PossibleMultipleEnumeration
        result.Should().BeEquivalentTo(strings);

        var arrayStrings = strings.ToArray();
        var arrayResult = result.ToArray();

        var sameOrder = true;
        for (var i = 0; i < arrayResult.Length; i++)
        {
            if (arrayResult[i] != arrayStrings[i])
            {
                sameOrder = false;

                break;
            }
        }

        sameOrder.Should().BeFalse();
    }

    [Fact]
    public void JoinWithFinalSeparator_should_use_final_separator_for_last_item()
    {
        var strings = new Fixture().CreateMany<string>(20);
        var result = strings.JoinWithFinalSeparator(", ", " and ");

        result.Should().Be(string.Join(", ", strings.Take(strings.Count() - 1)) + " and " + strings.Last());

        var items = new[] { 1, 2, 3 };
        items.JoinWithFinalSeparator(", ", " and ").Should().Be("1, 2 and 3");
    }

    [Fact]
    public void If_should_extend_the_query_with_a_provided_query_if_the_condition_is_met()
    {
        var items = new[] { 0, 1, 2, 3, 4, 5, 6, 7, 8, 9 };
        var result = items.If(true, q => q.Where(i => i > 3)).If(false, q => q.Where(i => i != 6)).If(true, q => q.Where(i => i < 7));

        result.Should().BeEquivalentTo(items.Where(i => i is > 3 and < 7));
    }

    [Fact]
    public void If_should_not_allow_null_query_action()
    {
        var items = new[] { 0, 1, 2, 3, 4, 5, 6, 7, 8, 9 };
#pragma warning disable CS8625 // Cannot convert null literal to non-nullable reference type.
        items.Invoking(i => i.If(true, null)).Should().Throw<ArgumentNullException>();
#pragma warning restore CS8625 // Cannot convert null literal to non-nullable reference type.
    }

    [Theory]
    [AutoMockData]
    public void ForEach_should_execute_action_on_each_element(int[] numbers)
    {
        var sum = 0;
        numbers.ForEach(n => sum += n);

        sum.Should().Be(numbers.Sum());
    }
}

public static class GenericCollectionAssertionsExtensions
{
    public static ICollection<T> ConvertOrCastToCollection<T>(this IEnumerable<T> source)
    {
        return source as ICollection<T> ?? source.ToList();
    }

    public static AndConstraint<TAssertions> ContainAllNotInOrder<TCollection, T, TAssertions>(this GenericCollectionAssertions<TCollection, T, TAssertions> assertions,
                                                                                               IEnumerable<T> expected,
                                                                                               string because = "",
                                                                                               params object[] becauseArgs)
        where TCollection : IEnumerable<T> where TAssertions : GenericCollectionAssertions<TCollection, T, TAssertions>
    {
        //Guard.ThrowIfArgumentIsNull(expected, nameof(expected), "Cannot verify containment against a <null> collection");

        var expectedObjects = expected.ConvertOrCastToCollection();
        // Guard.ThrowIfArgumentIsEmpty(expectedObjects, nameof(expected), "Cannot verify containment against an empty collection");

        bool success = Execute.Assertion.BecauseOf(because, becauseArgs)
                              .ForCondition(assertions.Subject is not null)
                              .FailWith("Expected {context:collection} to contain {0}{reason}, but found <null>.", expectedObjects);

        if (success)
        {
            var missingItems = expectedObjects.Where(item => !assertions.Subject.Contains(item));

            if (missingItems.Any())
            {
                if (expectedObjects.Count > 1)
                {
                    Execute.Assertion.BecauseOf(because, becauseArgs)
                           .FailWith("Expected {context:collection} {0} to contain {1}{reason}, but could not find {2}.",
                                     assertions.Subject,
                                     expectedObjects,
                                     missingItems);
                }
                else
                {
                    Execute.Assertion.BecauseOf(because, becauseArgs)
                           .FailWith("Expected {context:collection} {0} to contain {1}{reason}.", assertions.Subject, expectedObjects.Single());
                }
            }
        }

        return new AndConstraint<TAssertions>((TAssertions)assertions);
    }
}