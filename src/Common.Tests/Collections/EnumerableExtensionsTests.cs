using System;
using FluentAssertions;
using Ploch.Common.Collections;
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
    }
}