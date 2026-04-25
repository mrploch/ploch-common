using System;
using System.Collections;
using FluentAssertions.Equivalency;

namespace Ploch.TestingSupport.FluentAssertions;

/// <summary>
///     An <see cref="IEquivalencyStep" /> that treats a <see langword="null" /> collection
///     as equivalent to an empty collection (and vice versa).
/// </summary>
/// <remarks>
///     <para>
///         This step is useful when comparing object graphs where one side may have left a
///         collection property uninitialised (<see langword="null" />) while the other has
///         initialised it to an empty collection. A common scenario is EF Core entities: a
///         navigation collection that was not eager-loaded via <c>Include()</c> remains
///         <see langword="null" />, whereas in-memory test entities typically initialise it
///         to <c>new List&lt;T&gt;()</c>. Without this step, FluentAssertions treats the two
///         as different, producing false-negative assertion failures.
///     </para>
///     <para>
///         The step only intercedes when one side is <see langword="null" /> and the other is
///         an empty <see cref="IEnumerable" /> (excluding <see cref="string" />, which also
///         implements <see cref="IEnumerable" />). All other cases are passed through to the
///         next step in the pipeline, preserving configured options such as
///         <see cref="DateTimeOffset" /> tolerance and cyclic-reference handling.
///     </para>
///     <para>
///         Register the step via <c>options.Using(new NullEmptyCollectionEquivalencyStep())</c>
///         when calling <c>Should().BeEquivalentTo(...)</c>.
///     </para>
/// </remarks>
/// <example>
///     <code>
///     var actual = new Parent { Children = null };
///     var expected = new Parent { Children = new List&lt;Child&gt;() };
///
///     actual.Should().BeEquivalentTo(expected,
///         options => options.Using(new NullEmptyCollectionEquivalencyStep()));
///     </code>
/// </example>
public sealed class NullEmptyCollectionEquivalencyStep : IEquivalencyStep
{
    /// <inheritdoc />
    public EquivalencyResult Handle(Comparands comparands, IEquivalencyValidationContext context, IValidateChildNodeEquivalency valueChildNodes)
    {
        if (comparands.Subject is null && IsEmptyNonStringEnumerable(comparands.Expectation))
        {
            return EquivalencyResult.EquivalencyProven;
        }

        if (comparands.Expectation is null && IsEmptyNonStringEnumerable(comparands.Subject))
        {
            return EquivalencyResult.EquivalencyProven;
        }

        return EquivalencyResult.ContinueWithNext;
    }

    private static bool IsEmptyNonStringEnumerable(object? value)
    {
        if (value is string or null)
        {
            return false;
        }

        if (value is IEnumerable enumerable)
        {
            var enumerator = enumerable.GetEnumerator();
            try
            {
                return !enumerator.MoveNext();
            }
            finally
            {
                (enumerator as IDisposable)?.Dispose();
            }
        }

        return false;
    }
}
