using System;
using System.Collections.Generic;
using System.Linq;
using AutoFixture;
using FluentAssertions;
using Objectivity.AutoFixture.XUnit2.AutoMoq.Attributes;
using Ploch.Common.Collections;
using Ploch.Common.Tests.Reflection;
using Xunit;

namespace Ploch.Common.Tests.Collections
{
    public class EnumerableExtensionsTests
    {
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
            var list = new List<string>();
            for (var i = 0; i < 1000; i++)
            {
                list.Add($"Item {i}");
            }

            var result = list.TakeRandom(200);

            result.Should().HaveCount(200);
        }

        [Theory]
        [AutoMockData]
        public void FirstOrProvided_should_return_provided_value_if_item_is_not_found(IEnumerable<string> strings)
        {
            var expected = "MyValue";

            var result = strings.FirstOrProvided(s => s == Guid.NewGuid().ToString(), () => expected);

            result.Should().Be(expected);
        }

        [Theory]
        [AutoMockData]
        public void FirstOrProvided_should_return_item_if_found(List<string> strings)
        {
            var expected = "MyValue";
            strings.Add(expected);
            var result = strings.FirstOrProvided(s => s == expected, () => "NotExpectedValue");

            result.Should().Be(expected);
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
    }
}