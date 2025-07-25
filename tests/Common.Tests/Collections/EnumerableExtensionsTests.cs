using System.Collections;
using AutoFixture;
using FluentAssertions;
using Objectivity.AutoFixture.XUnit2.AutoMoq.Attributes;
using Ploch.Common.Collections;
using Ploch.Common.Tests.TestTypes.TestingTypes;
using Xunit.Abstractions;

namespace Ploch.Common.Tests.Collections;

public class EnumerableExtensionsTests(ITestOutputHelper output)
{
    private static readonly int[] Items = [ 1, 2, 3 ];

    [Fact]
    public void ValueIn_should_work_on_short_type()
    {
        short number = 10;

        number.ValueIn<short>(1, 2, 3, 4, 5, 6, 7, 8, 9, 10).Should().BeTrue();
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
#pragma warning disable CS8600 // Converting null literal or possible null value to non-nullable type.
        ((object)null).ValueIn(new object(), new object(), null).Should().BeTrue();
        ((string)null).ValueIn("str1", "str2").Should().BeFalse();
#pragma warning restore CS8600 // Converting null literal or possible null value to non-nullable type.
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
        var testObjects = new MyTestClass[] { new() { IntProp = 1 }, new() { IntProp = 2 }, new() { IntProp = 3 } };

        var join = testObjects.JoinWithFinalSeparator(", ", " and ", o => o.IntProp);

        join.Should().Be("1, 2 and 3");
    }

    [Fact]
    public void TakeRandom_should_pick_count_number_of_random_items_from_source()
    {
        var (sourceList, result) = TestTakeRandom(100, 10);

        result.Should().HaveCount(10).And.OnlyContain(s => sourceList.Contains(s)).And.NotContainInOrder(sourceList.Take(10));
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
        var (_, result) = TestTakeRandom(10, 0);

        result.Should().BeEmpty();
    }

    [Fact]
    public void TakeRandom_should_return_0_items_if_source_list_size_is_0()
    {
        var (_, result) = TestTakeRandom(0, 10);

        result.Should().BeEmpty();
    }

    [Fact]
    public void TakeRandom_should_return_random_values_from_source()
    {
        var results = new List<IEnumerable<int>>();

        var sourceList = new List<int>();
        for (var i = 0; i < 100; i++)
        {
            sourceList.Add(i);
        }

        for (var i = 0; i < 10; i++)
        {
            var randomValues = sourceList.TakeRandom(3).ToList();

            results.Should().NotContainEquivalentOf(randomValues);

            results.Add(randomValues);
        }
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

    [Fact]
    public void AreIntegersSequentialInOrder_returns_true_if_numbers_in_a_sequence_are_sequential()
    {
        var sequential = new[] { 1, 2, 3, 4, 5, 6, 7 };
        var sequentialNotInOrderNumbers = new[] { 1, 3, 2, 4, 5, 6, 7 };
        var nonSequentialNumbers = new[] { 1, 2, 3, 4, 5, 6, 8 };

        sequential.AreIntegersSequentialInOrder().Should().BeTrue();
        sequentialNotInOrderNumbers.AreIntegersSequentialInOrder().Should().BeFalse();
        nonSequentialNumbers.AreIntegersSequentialInOrder().Should().BeFalse();
    }

    [Fact]
    public void AreIntegersSequentialInOrder_returns_true_if_numbers_in_a_sequence_are_sequential_for_longs()
    {
        var sequential = new long[] { 1, 2, 3, 4, 5, 6, 7 };
        var sequentialNotInOrderNumbers = new long[] { 1, 3, 2, 4, 5, 6, 7 };
        var nonSequentialNumbers = new long[] { 1, 2, 3, 4, 5, 6, 8 };

        sequential.AreIntegersSequentialInOrder().Should().BeTrue();
        sequentialNotInOrderNumbers.AreIntegersSequentialInOrder().Should().BeFalse();
        nonSequentialNumbers.AreIntegersSequentialInOrder().Should().BeFalse();
    }

    [Fact]
    public void IsEmpty_should_throw_ArgumentNullException_if_enumerable_is_null()
    {
#pragma warning disable CS8600 // Converting null literal or possible null value to non-nullable type.
        IEnumerable<string> list = null;
#pragma warning restore CS8600 // Converting null literal or possible null value to non-nullable type.

        var act = () => list.IsEmpty();

        act.Should().Throw<ArgumentNullException>();
    }

    [Fact]
    public void IsEmpty_should_return_true_if_enumerable_has_no_elements()
    {
        // ReSharper disable once CollectionNeverUpdated.Local
        var list = new List<string>();

        list.IsEmpty().Should().BeTrue();
    }

    [Theory]
    [AutoMockData]
    public void IsEmpty_should_return_false_if_enumerable_contains_elements(List<string> list)
    {
        list.IsEmpty().Should().BeFalse();
    }

    [Fact]
    public void IsNullOrEmpty_should_return_true_for_null_enumerable()
    {
        IEnumerable<int>? nullEnumerable = null;
        nullEnumerable.IsNullOrEmpty().Should().BeTrue();
    }

    [Fact]
    public void IsNullOrEmpty_should_return_true_for_empty_enumerable()
    {
        var emptyEnumerable = Enumerable.Empty<int>();
        emptyEnumerable.IsNullOrEmpty().Should().BeTrue();
    }

    [Fact]
    public void IsNullOrEmpty_should_return_false_for_non_empty_enumerable()
    {
#pragma warning disable CC0001
        IEnumerable<int> nonEmptyEnumerable = [ 1, 2, 3 ];
#pragma warning restore CC0001
        nonEmptyEnumerable.IsNullOrEmpty().Should().BeFalse();
    }

    [Fact]
    public void Second_should_return_second_element_in_sequence()
    {
        IEnumerable<int> nonEmptyEnumerable = [ 1, 2, 3 ];

        nonEmptyEnumerable.Second().Should().Be(2);
    }

    [Fact]
    public void Second_should_throw_argument_null_exception_if_sequence_is_null()
    {
        // Arrange
        IEnumerable<int>? sequence = null;

        // Act
#pragma warning disable CS8604 // Possible null reference argument.
        Action act = () => sequence.Second();
#pragma warning restore CS8604 // Possible null reference argument.

        // Assert
        act.Should().Throw<ArgumentNullException>();
    }

    [Fact]
    public void Second_should_throw_if_less_items_in_sequence_than_two()
    {
        // Arrange
        var singleItemList = new List<int> { 1 };

        // ReSharper disable once CollectionNeverUpdated.Local
        var emptyList = new List<int>();

        // Act
        Action actSingleItem = () => singleItemList.Second();
        Action actEmpty = () => emptyList.Second();

        // Assert
        actSingleItem.Should().Throw<InvalidOperationException>().WithMessage("Sequence*");
        actEmpty.Should().Throw<InvalidOperationException>().WithMessage("Sequence*");
    }

    [Fact]
    public void IsEmpty_should_dispose_IDisposable_enumerator()
    {
        // Arrange
        var mockDisposableEnumerable = new DisposableEnumerableStub(Items);

        // Act
        var isEmpty = mockDisposableEnumerable.IsEmpty();

        // Assert
        isEmpty.Should().BeFalse();
        mockDisposableEnumerable.WasDisposed.Should().BeTrue();
    }

    [Fact]
    public void IsEmpty_should_return_true_for_empty_non_generic_array()
    {
        // Arrange
        Array emptyArray = Array.Empty<int>();

        // Act
        var isEmpty = emptyArray.IsEmpty();

        // Assert
        isEmpty.Should().BeTrue();
    }

    [Fact]
    public void IsEmpty_should_work_with_custom_enumerables_that_dont_implement_IDisposable()
    {
        // Arrange
        var nonEmptyEnumerable = new NonDisposableEnumerableStub(Items);
        var emptyEnumerable = new NonDisposableEnumerableStub(Array.Empty<int>());

        // Act
        var nonEmptyResult = nonEmptyEnumerable.IsEmpty();
        var emptyResult = emptyEnumerable.IsEmpty();

        // Assert
        nonEmptyResult.Should().BeFalse();
        emptyResult.Should().BeTrue();
    }

    [Fact]
    public void IsEmpty_should_work_with_types_implementing_both_IEnumerable_and_IDisposable_directly()
    {
        // Arrange
        var nonEmptyEnumerable = new EnumerableAndDisposableStub(Items);
        var emptyEnumerable = new EnumerableAndDisposableStub(Array.Empty<int>());

        // Act
        var nonEmptyResult = nonEmptyEnumerable.IsEmpty();
        var emptyResult = emptyEnumerable.IsEmpty();

        // Assert
        nonEmptyResult.Should().BeFalse();
        emptyResult.Should().BeTrue();
        nonEmptyEnumerable.WasDisposed.Should().BeTrue();
        emptyEnumerable.WasDisposed.Should().BeTrue();
    }

    [Fact]
    public void IsEmpty_should_properly_handle_string_enumeration()
    {
        // Arrange
        var emptyString = string.Empty;
        var nonEmptyString = "Hello, world!";

        // Act
        var emptyResult = emptyString.IsEmpty();
        var nonEmptyResult = nonEmptyString.IsEmpty();

        // Assert
        emptyResult.Should().BeTrue();
        nonEmptyResult.Should().BeFalse();
    }

    [Fact]
    public void IsEmpty_should_work_correctly_with_yield_return_based_enumerables()
    {
        // Arrange
        var emptyYieldEnumerable = YieldEmptyEnumerable();
        var nonEmptyYieldEnumerable = YieldNonEmptyEnumerable();

        // Act
        var emptyResult = emptyYieldEnumerable.IsEmpty();
        var nonEmptyResult = nonEmptyYieldEnumerable.IsEmpty();

        // Assert
        emptyResult.Should().BeTrue();
        nonEmptyResult.Should().BeFalse();
    }

    [Fact]
    public void IsEmpty_should_work_with_collections_implementing_multiple_enumerable_interfaces()
    {
        // Arrange
        var multipleInterfacesCollection = new MultipleEnumerableInterfacesCollection(Items);
        var emptyMultipleInterfacesCollection = new MultipleEnumerableInterfacesCollection([]);

        // Act
        var nonEmptyResult = multipleInterfacesCollection.IsEmpty();
        var emptyResult = emptyMultipleInterfacesCollection.IsEmpty();

        // Assert
        nonEmptyResult.Should().BeFalse();
        emptyResult.Should().BeTrue();
    }

    private static IEnumerable YieldEmptyEnumerable()
    {
        yield break;
    }

    private static IEnumerable YieldNonEmptyEnumerable()
    {
        yield return 1;
        yield return 2;
        yield return 3;
    }

    private (List<string> list, IEnumerable<string> result) TestTakeRandom(int itemsCount, int sampleSize)
    {
        var list = new List<string>();
        for (var i = 0; i < itemsCount; i++)
        {
            list.Add($"Item {i}");
        }

        var result = list.TakeRandom(sampleSize);
        output.WriteLine($"TakeRandom test. Sample size: {sampleSize}, items count: {itemsCount}, result count: {result.Count()}");

        return (list, result);
    }

    private class DisposableEnumerableStub(IEnumerable items) : IEnumerable
    {
        public bool WasDisposed { get; private set; }

        public IEnumerator GetEnumerator()
        {
            // ReSharper disable once GenericEnumeratorNotDisposed
            return new DisposableEnumeratorStub(items.GetEnumerator(), () => WasDisposed = true);
        }

        private class DisposableEnumeratorStub(IEnumerator innerEnumerator, Action onDispose) : IEnumerator, IDisposable
        {
            public void Dispose()
            {
                onDispose();
                (innerEnumerator as IDisposable)?.Dispose();
            }

            public object? Current => innerEnumerator.Current;

            public bool MoveNext() => innerEnumerator.MoveNext();

            public void Reset() => innerEnumerator.Reset();
        }
    }

    private class NonDisposableEnumerableStub(IEnumerable items) : IEnumerable
    {
        // ReSharper disable once GenericEnumeratorNotDisposed
        public IEnumerator GetEnumerator() => new NonDisposableEnumeratorStub(items.GetEnumerator());

        private class NonDisposableEnumeratorStub(IEnumerator innerEnumerator) : IEnumerator
        {
            public object? Current => innerEnumerator.Current;

            public bool MoveNext() => innerEnumerator.MoveNext();

            public void Reset() => innerEnumerator.Reset();
        }
    }

    private class EnumerableAndDisposableStub : IEnumerable, IDisposable, IEnumerator
    {
        private readonly IEnumerator _enumerator;
        private readonly IEnumerable _items;

        public EnumerableAndDisposableStub(IEnumerable items)
        {
            _items = items;
            _enumerator = _items.GetEnumerator();
        }

        public bool WasDisposed { get; private set; }

        public void Dispose() => WasDisposed = true;

        public IEnumerator GetEnumerator() => this;

        public object? Current => _enumerator.Current;

        public bool MoveNext() => _enumerator.MoveNext();

        public void Reset() => _enumerator.Reset();
    }

    private class MultipleEnumerableInterfacesCollection(int[] items) : IReadOnlyCollection<int>
    {
        public int Count => items.Length;

        public IEnumerator<int> GetEnumerator() => ((IEnumerable<int>)items).GetEnumerator();

        IEnumerator IEnumerable.GetEnumerator() => items.GetEnumerator();
    }
}
