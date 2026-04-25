using System.Collections.Generic;
using FluentAssertions.Equivalency;

namespace Ploch.TestingSupport.FluentAssertions.Tests;

public class NullEmptyCollectionEquivalencyStepTests
{
    [Fact]
    public void BeEquivalentTo_should_treat_null_collection_as_equivalent_to_empty_collection()
    {
        var actual = new Holder { Items = null };
        var expected = new Holder { Items = new List<int>() };

        var act = () => actual.Should().BeEquivalentTo(expected, options => options.Using(new NullEmptyCollectionEquivalencyStep()));

        act.Should().NotThrow();
    }

    [Fact]
    public void BeEquivalentTo_should_treat_empty_collection_as_equivalent_to_null_collection()
    {
        var actual = new Holder { Items = new List<int>() };
        var expected = new Holder { Items = null };

        var act = () => actual.Should().BeEquivalentTo(expected, options => options.Using(new NullEmptyCollectionEquivalencyStep()));

        act.Should().NotThrow();
    }

    [Fact]
    public void BeEquivalentTo_should_pass_when_both_collections_are_null()
    {
        var actual = new Holder { Items = null };
        var expected = new Holder { Items = null };

        var act = () => actual.Should().BeEquivalentTo(expected, options => options.Using(new NullEmptyCollectionEquivalencyStep()));

        act.Should().NotThrow();
    }

    [Fact]
    public void BeEquivalentTo_should_pass_when_both_collections_are_empty()
    {
        var actual = new Holder { Items = new List<int>() };
        var expected = new Holder { Items = new List<int>() };

        var act = () => actual.Should().BeEquivalentTo(expected, options => options.Using(new NullEmptyCollectionEquivalencyStep()));

        act.Should().NotThrow();
    }

    [Fact]
    public void BeEquivalentTo_should_fail_when_subject_is_null_but_expected_is_populated()
    {
        var actual = new Holder { Items = null };
        var expected = new Holder { Items = new List<int> { 1, 2, 3 } };

        var act = () => actual.Should().BeEquivalentTo(expected, options => options.Using(new NullEmptyCollectionEquivalencyStep()));

        act.Should().Throw<XunitException>();
    }

    [Fact]
    public void BeEquivalentTo_should_fail_when_expected_is_null_but_subject_is_populated()
    {
        var actual = new Holder { Items = new List<int> { 1, 2, 3 } };
        var expected = new Holder { Items = null };

        var act = () => actual.Should().BeEquivalentTo(expected, options => options.Using(new NullEmptyCollectionEquivalencyStep()));

        act.Should().Throw<XunitException>();
    }

    [Fact]
    public void BeEquivalentTo_should_pass_populated_collections_through_to_default_equivalency()
    {
        var actual = new Holder { Items = new List<int> { 1, 2, 3 } };
        var expected = new Holder { Items = new List<int> { 1, 2, 3 } };

        var act = () => actual.Should().BeEquivalentTo(expected, options => options.Using(new NullEmptyCollectionEquivalencyStep()));

        act.Should().NotThrow();
    }

    [Fact]
    public void BeEquivalentTo_should_fail_when_populated_collections_have_different_items()
    {
        var actual = new Holder { Items = new List<int> { 1, 2, 3 } };
        var expected = new Holder { Items = new List<int> { 1, 2, 4 } };

        var act = () => actual.Should().BeEquivalentTo(expected, options => options.Using(new NullEmptyCollectionEquivalencyStep()));

        act.Should().Throw<XunitException>();
    }

    [Fact]
    public void BeEquivalentTo_should_not_treat_null_string_as_equivalent_to_empty_string()
    {
        // Guards against an easy-to-miss bug: string implements IEnumerable<char>, so without
        // an explicit exclusion the step would silently equate null and "".
        var actual = new Holder { Items = new List<int>(), Label = null };
        var expected = new Holder { Items = new List<int>(), Label = string.Empty };

        var act = () => actual.Should().BeEquivalentTo(expected, options => options.Using(new NullEmptyCollectionEquivalencyStep()));

        act.Should().Throw<XunitException>();
    }

    [Fact]
    public void BeEquivalentTo_should_treat_null_array_as_equivalent_to_empty_array()
    {
        var actual = new ArrayHolder { Values = null };
        var expected = new ArrayHolder { Values = System.Array.Empty<string>() };

        var act = () => actual.Should().BeEquivalentTo(expected, options => options.Using(new NullEmptyCollectionEquivalencyStep()));

        act.Should().NotThrow();
    }

    [Fact]
    public void BeEquivalentTo_should_treat_null_dictionary_as_equivalent_to_empty_dictionary()
    {
        var actual = new DictionaryHolder { Map = null };
        var expected = new DictionaryHolder { Map = new Dictionary<string, int>() };

        var act = () => actual.Should().BeEquivalentTo(expected, options => options.Using(new NullEmptyCollectionEquivalencyStep()));

        act.Should().NotThrow();
    }

    [Fact]
    public void Handle_should_return_ContinueWithNext_when_neither_side_is_null()
    {
        var step = new NullEmptyCollectionEquivalencyStep();
        var comparands = new Comparands(new List<int> { 1 }, new List<int> { 1 }, typeof(List<int>));

        var result = step.Handle(comparands, context: null!, valueChildNodes: null!);

        result.Should().Be(EquivalencyResult.ContinueWithNext);
    }

    [Fact]
    public void Handle_should_return_EquivalencyProven_for_null_subject_and_empty_expectation()
    {
        var step = new NullEmptyCollectionEquivalencyStep();
        var comparands = new Comparands(subject: null!, new List<int>(), typeof(List<int>));

        var result = step.Handle(comparands, context: null!, valueChildNodes: null!);

        result.Should().Be(EquivalencyResult.EquivalencyProven);
    }

    [Fact]
    public void Handle_should_return_ContinueWithNext_for_null_subject_and_populated_expectation()
    {
        var step = new NullEmptyCollectionEquivalencyStep();
        var comparands = new Comparands(subject: null!, new List<int> { 1 }, typeof(List<int>));

        var result = step.Handle(comparands, context: null!, valueChildNodes: null!);

        result.Should().Be(EquivalencyResult.ContinueWithNext);
    }

    private sealed class Holder
    {
        public List<int>? Items { get; set; }

        public string? Label { get; set; }
    }

    private sealed class ArrayHolder
    {
        public string[]? Values { get; set; }
    }

    private sealed class DictionaryHolder
    {
        public Dictionary<string, int>? Map { get; set; }
    }
}
