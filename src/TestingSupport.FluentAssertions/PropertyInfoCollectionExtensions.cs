using System.Collections.Generic;
using System.Reflection;
using FluentAssertions.Execution;

namespace Ploch.TestingSupport.FluentAssertions;

/// <summary>
///     Provides FluentAssertions extension methods for collections of <see cref="PropertyInfo" /> instances.
/// </summary>
public static class PropertyInfoCollectionExtensions
{
    /// <summary>
    ///     Returns a <see cref="PropertyInfoCollectionAssertions" /> object that can be used to assert the supplied collection of properties.
    /// </summary>
    /// <param name="instance">The collection of properties to assert against.</param>
    /// <returns>A <see cref="PropertyInfoCollectionAssertions" /> for the supplied collection.</returns>
    public static PropertyInfoCollectionAssertions Should(this IEnumerable<PropertyInfo> instance) => new(instance, AssertionChain.GetOrCreate());
}
