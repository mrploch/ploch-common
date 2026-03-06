using System.Collections.Generic;
using System.Reflection;
using FluentAssertions.Execution;

namespace Ploch.TestingSupport.FluentAssertions;

public static class PropertyInfoCollectionExtensions
{
    public static PropertyInfoCollectionAssertions Should(this IEnumerable<PropertyInfo> instance) => new(instance, AssertionChain.GetOrCreate());
}
