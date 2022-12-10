using System;
using System.Collections.Generic;
using System.Linq;
using FluentAssertions;
using Ploch.Common.Collections;
using Ploch.Common.Tests.Reflection;
using Ploch.TestingSupport.Xunit.AutoFixture;
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

        [Theory]
        [AutoDataMoq]
        public void Join_should_string_join_int_enumerable(IEnumerable<int> ints, string separator)
        {
            ints.Join(separator).Should().Be(string.Join(separator, ints));
        }

        [Theory]
        [AutoDataMoq]
        public void Join_should_string_join_datetime_enumerable(IEnumerable<DateTime> dateTimes, string separator)
        {
            var join = dateTimes.Join(separator);
            join.Should().Be(string.Join(separator, dateTimes));

            join = dateTimes.Join(" ", dt => dt.Year);
            join.Should().Be(string.Join(" ", dateTimes.Select(dt => dt.Year)));
        }

        [Theory]
        [AutoDataMoq]
        public void Join_should_string_join_objects_enumerable_with_different_final_separator()
        {
            var testObjects = new TestTypes.MyTestClass[] { new() { IntProp = 1 }, new() { IntProp = 2 }, new() { IntProp = 3 } };

            var join = testObjects.JoinWithFinalSeparator(", ", " and ", o => o.IntProp);

            join.Should().Be("1, 2 and 3");
        }
    }
}