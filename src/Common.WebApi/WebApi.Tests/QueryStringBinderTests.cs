using System.Globalization;
using FluentAssertions;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Primitives;

namespace Ploch.Common.WebApi.Tests;

public class QueryStringBinderTests
{
    [Fact]
    public void TryParse_should_bind_a_string_property()
    {
        var succeeded = QueryStringBinder.TryParse<TestQuery>(Query(("Name", "widgets")), out var query);

        succeeded.Should().BeTrue();
        query.Name.Should().Be("widgets");
    }

    [Fact]
    public void TryParse_should_bind_an_int_property()
    {
        QueryStringBinder.TryParse<TestQuery>(Query(("Page", "42")), out var query).Should().BeTrue();

        query.Page.Should().Be(42);
    }

    [Fact]
    public void TryParse_should_bind_a_nullable_value_type_property()
    {
        QueryStringBinder.TryParse<TestQuery>(Query(("OptionalPage", "7")), out var query).Should().BeTrue();

        query.OptionalPage.Should().Be(7);
    }

    [Fact]
    public void TryParse_should_bind_an_enum_property()
    {
        QueryStringBinder.TryParse<TestQuery>(Query(("Direction", "Descending")), out var query).Should().BeTrue();

        query.Direction.Should().Be(SortDirection.Descending);
    }

    [Fact]
    public void TryParse_should_bind_a_bool_property()
    {
        QueryStringBinder.TryParse<TestQuery>(Query(("IncludeArchived", "true")), out var query).Should().BeTrue();

        query.IncludeArchived.Should().BeTrue();
    }

    [Fact]
    public void TryParse_should_bind_every_supported_property_in_a_single_pass()
    {
        var succeeded = QueryStringBinder.TryParse<TestQuery>(Query(("Name", "widgets"),
                                                                   ("Page", "2"),
                                                                   ("OptionalPage", "3"),
                                                                   ("IncludeArchived", "true"),
                                                                   ("CreatedOn", "2024-03-04T10:30:00"),
                                                                   ("UpdatedOn", "2024-03-04T10:30:00+00:00"),
                                                                   ("StartDate", "2024-03-04"),
                                                                   ("StartTime", "10:30"),
                                                                   ("Direction", "Descending")),
                                                             out var query);

        succeeded.Should().BeTrue();
        query.Name.Should().Be("widgets");
        query.Page.Should().Be(2);
        query.OptionalPage.Should().Be(3);
        query.IncludeArchived.Should().BeTrue();
        query.CreatedOn.Should().Be(new DateTime(2024, 3, 4, 10, 30, 0, DateTimeKind.Unspecified));
        query.UpdatedOn.Should().Be(new DateTimeOffset(2024, 3, 4, 10, 30, 0, TimeSpan.Zero));
        query.StartDate.Should().Be(new DateOnly(2024, 3, 4));
        query.StartTime.Should().Be(new TimeOnly(10, 30));
        query.Direction.Should().Be(SortDirection.Descending);
    }

    // Regression: the binder previously used the IDictionary indexer, which threw
    // KeyNotFoundException whenever a bound type had a property absent from the query string.
    [Fact]
    public void TryParse_should_leave_properties_absent_from_the_query_at_their_default_value()
    {
        var act = () => QueryStringBinder.TryParse<TestQuery>(Query(("Name", "widgets")), out _);

        act.Should().NotThrow();

        QueryStringBinder.TryParse<TestQuery>(Query(("Name", "widgets")), out var query).Should().BeTrue();
        query.Page.Should().Be(0);
        query.OptionalPage.Should().BeNull();
        query.Direction.Should().Be(SortDirection.Ascending);
    }

    [Fact]
    public void TryParse_should_skip_a_key_present_with_no_values()
    {
        var query = new Dictionary<string, StringValues> { ["Page"] = StringValues.Empty };

        QueryStringBinder.TryParse<TestQuery>(query, out var result).Should().BeTrue();

        result.Page.Should().Be(0);
    }

    [Fact]
    public void TryParse_should_skip_a_key_whose_first_value_is_null()
    {
        var query = new Dictionary<string, StringValues> { ["Name"] = new StringValues((string?)null) };

        var act = () => QueryStringBinder.TryParse<TestQuery>(query, out _);

        act.Should().NotThrow();
    }

    [Fact]
    public void TryParse_should_return_false_when_a_property_type_is_not_supported()
    {
        QueryStringBinder.TryParse<UnsupportedQuery>(Query((nameof(UnsupportedQuery.Endpoint), "https://example.test")), out _).Should().BeFalse();
    }

    // Regression: parsing previously used the ambient culture, so a query string is
    // interpreted differently depending on the server's locale.
    [Fact]
    public void TryParse_should_parse_dates_using_the_invariant_culture_regardless_of_the_ambient_culture()
    {
        var original = CultureInfo.CurrentCulture;
        try
        {
            // en-GB reads 03/04/2024 as 3 April; the invariant culture reads it as 4 March.
            CultureInfo.CurrentCulture = new CultureInfo("en-GB");

            QueryStringBinder.TryParse<TestQuery>(Query(("CreatedOn", "03/04/2024")), out var query).Should().BeTrue();

            query.CreatedOn.Should().Be(new DateTime(2024, 3, 4, 0, 0, 0, DateTimeKind.Unspecified));
        }
        finally
        {
            CultureInfo.CurrentCulture = original;
        }
    }

    [Fact]
    public void Bind_should_bind_the_query_string_of_the_http_context()
    {
        var httpContext = new DefaultHttpContext();
        httpContext.Request.QueryString = new QueryString("?Name=widgets&Page=5");

        var query = QueryStringBinder.Bind<TestQuery>(httpContext);

        query.Name.Should().Be("widgets");
        query.Page.Should().Be(5);
    }

    [Fact]
    public void Bind_should_throw_NotSupportedException_when_a_property_type_is_not_supported()
    {
        var httpContext = new DefaultHttpContext();
        httpContext.Request.QueryString = new QueryString("?Endpoint=https%3A%2F%2Fexample.test");

        var act = () => QueryStringBinder.Bind<UnsupportedQuery>(httpContext);

        act.Should().Throw<NotSupportedException>();
    }

    private static Dictionary<string, StringValues> Query(params (string Key, string Value)[] values) =>
        values.ToDictionary(v => v.Key, v => new StringValues(v.Value));
}
