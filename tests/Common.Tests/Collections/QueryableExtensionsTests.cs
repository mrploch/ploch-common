using FluentAssertions;
using Ploch.Common.Collections;
using Xunit;

namespace Ploch.Common.Tests.Collections;

public class QueryableExtensionsTests
{
    [Fact]
    public void If_should_extend_the_query_with_a_provided_query_if_the_condition_is_met()
    {
        var items = new[] { 0, 1, 2, 3, 4, 5, 6, 7, 8, 9 }.AsQueryable();
        var actual = items.If(true, q => q.Where(i => i > 3)).If(false, q => q.Where(i => i != 6)).If(true, q => q.Where(i => i < 7));
        var expected = items.Where(i => i > 3 && i < 7).AsQueryable();
        actual.Should().BeEquivalentTo(expected);
    }

    [Fact]
    public void If_should_not_allow_null_query_action()
    {
        var items = new[] { 0, 1, 2, 3, 4, 5, 6, 7, 8, 9 }.AsQueryable();
#pragma warning disable CS8625 // Cannot convert null literal to non-nullable reference type.
        items.Invoking(i => i.If(true, null)).Should().Throw<ArgumentNullException>();
#pragma warning restore CS8625 // Cannot convert null literal to non-nullable reference type.
    }
}
