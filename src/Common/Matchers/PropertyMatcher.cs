using System;

namespace Ploch.Common.Matchers;

public class PropertyMatcher<TSourceType>(Func<TSourceType, string> propertySelector, IStringMatcher stringMatcher) : IMatcher<TSourceType>
{
    public bool IsMatch(TSourceType obj) => stringMatcher.IsMatch(propertySelector(obj));
}
