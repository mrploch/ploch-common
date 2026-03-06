using System;

namespace Ploch.Common.Matchers;

/// <summary>
///     A matcher that evaluates a property of an object against a string matcher.
/// </summary>
/// <typeparam name="TSourceType">The type of the object containing the property to match.</typeparam>
/// <param name="propertySelector">A function that extracts the string property from the source object.</param>
/// <param name="stringMatcher">The string matcher used to evaluate the extracted property value.</param>
public class PropertyMatcher<TSourceType>(Func<TSourceType?, string?> propertySelector, IMatcher<string?> stringMatcher) : IMatcher<TSourceType>
{
    /// <summary>
    ///     Determines whether the specified value matches the criteria defined by this matcher.
    /// </summary>
    /// <param name="value">The object to evaluate.</param>
    /// <returns>
    ///     <c>true</c> if the property extracted from the value matches the criteria defined by the string matcher; otherwise, <c>false</c>.
    /// </returns>
    public bool IsMatch(TSourceType? value) => stringMatcher.IsMatch(propertySelector(value));
}
