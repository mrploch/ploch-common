using Ploch.Common.Collections;

namespace Ploch.Common.Tests.Collections;
#if NET7_0_OR_GREATER
public class EnumerableQueriesTests
{
    [Fact]
    public void GetWithEmptyProperty_should_return_items_with_null_property()
    {
        var items = new[] { new TestItem("1", null), new TestItem("2", "not null") };
        var result = items.GetWithEmptyProperty(x => x.Value2);
        result.Should().ContainSingle(ti => ti.Value1 == "1" && ti.Value2 == null);
    }

    [Fact]
    public void GetWithEmptyProperty_should_return_empty_collection_when_input_is_empty()
    {
        var emptyCollection = Array.Empty<TestItem>();
        var result = emptyCollection.GetWithEmptyProperty(x => x.Value2);
        result.Should().BeEmpty();
    }

    [Fact]
    public void GetWithEmptyProperty_should_return_items_with_empty_string_properties()
    {
        var items = new[] { new TestItem("1", string.Empty), new TestItem("2", "not empty"), new TestItem("3", "") };
        var result = items.GetWithEmptyProperty(x => x.Value2);
        result.Should().HaveCount(2);
        result.Should().Contain(ti => ti.Value1 == "1" && ti.Value2 == string.Empty);
        result.Should().Contain(ti => ti.Value1 == "3" && ti.Value2 == "");
    }

    private record TestItem(string? Value1, string? Value2);
}
#endif
