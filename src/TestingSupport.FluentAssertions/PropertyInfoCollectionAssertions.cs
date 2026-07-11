using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using FluentAssertions;
using FluentAssertions.Collections;
using FluentAssertions.Execution;

namespace Ploch.TestingSupport.FluentAssertions;

/// <summary>
///     Provides assertions for collections of <see cref="PropertyInfo" /> instances.
/// </summary>
/// <param name="actualValue">The collection of properties being asserted against.</param>
/// <param name="chain">The assertion chain used to record the outcome of the assertions.</param>
public class PropertyInfoCollectionAssertions(IEnumerable<PropertyInfo> actualValue, AssertionChain chain)
    : GenericCollectionAssertions<PropertyInfo>(actualValue, chain)
{
    /// <inheritdoc />
    protected override string Identifier => "properties";

    /// <summary>
    ///     Asserts that the collection contains the property with the specified name declared on the supplied source object.
    /// </summary>
    /// <param name="propertyName">The name of the property the collection is expected to contain.</param>
    /// <param name="sourceObj">The object whose type declares the expected property.</param>
    /// <param name="because">A formatted phrase explaining why the assertion is needed.</param>
    /// <param name="becauseArgs">Zero or more objects to format using the placeholders in <paramref name="because" />.</param>
    /// <returns>An <see cref="AndConstraint{T}" /> that can be used to chain further assertions.</returns>
    [CustomAssertion]
    public AndConstraint<PropertyInfoCollectionAssertions> ContainProperty(string propertyName,
                                                                           object sourceObj,
                                                                           string because = "",
                                                                           params object[] becauseArgs)
    {
        CurrentAssertionChain.BecauseOf(because, becauseArgs)
                             .ForCondition(!string.IsNullOrEmpty(propertyName) || sourceObj is not null)
                             .FailWith("You can't assert that collection contains a property if you don't pass a proper name and source object")
                             .Then.Given(() => Subject)
                             .ForCondition(properties => properties.Any(fileInfo => fileInfo == sourceObj!.GetType().GetProperty(propertyName)))
                             .FailWith("Expected {context:properties} to contain {0}{reason}, but found {1}.",
                                       _ => propertyName,
                                       properties => properties.Select(propertyInfo => propertyInfo.Name));

        return new(this);
    }

    /// <summary>
    ///     Asserts that the collection contains all properties with the specified names declared on the supplied source object.
    /// </summary>
    /// <param name="propertyNames">The names of the properties the collection is expected to contain.</param>
    /// <param name="sourceObj">The object whose type declares the expected properties.</param>
    /// <param name="because">A formatted phrase explaining why the assertion is needed.</param>
    /// <param name="becauseArgs">Zero or more objects to format using the placeholders in <paramref name="because" />.</param>
    /// <returns>An <see cref="AndConstraint{T}" /> that can be used to chain further assertions.</returns>
    [CustomAssertion]
    public AndConstraint<PropertyInfoCollectionAssertions> ContainProperties(string[] propertyNames,
                                                                             object sourceObj,
                                                                             string because = "",
                                                                             params object[] becauseArgs)
    {
        CurrentAssertionChain.BecauseOf(because, becauseArgs)
                             .ForCondition(propertyNames.Length > 0 && sourceObj is not null)

                             // .FailWith("You can't assert that collection contains a property if you don't pass a proper name and source object")
                             .FailWith(GetFailureMessage(propertyNames, sourceObj))
                             .Then.Given(() => Subject)
                             .ForCondition(properties =>
                                           {
                                               return propertyNames.All(propertyName =>
                                                                            properties.Any(fileInfo => fileInfo ==
                                                                                                       sourceObj!.GetType().GetProperty(propertyName)));
                                           })
                             .FailWith("Expected {context:properties} to contain {0}{reason}, but found {1}.",
                                       _ => propertyNames,
                                       properties => properties.Select(propertyInfo => propertyInfo.Name));

        return new(this);
    }

    private static string GetFailureMessage(string[] propertyNames, object? sourceObj)
    {
        if (propertyNames.Length > 0 && sourceObj is not null)
        {
            return string.Empty;
        }

        var sb = new StringBuilder("You have to provide ");
        var addAnd = false;
        if (propertyNames.Length == 0)
        {
            sb.Append("at least one property name");
            addAnd = true;
        }

        if (sourceObj is null)
        {
            if (addAnd)
            {
                sb.Append(" and ");
            }

            sb.Append("a source object that is not null");
        }

        return sb.ToString();
    }
}
