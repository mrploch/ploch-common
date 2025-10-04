using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using FluentAssertions;
using FluentAssertions.Collections;
using FluentAssertions.Execution;

namespace Ploch.TestingSupport.FluentAssertions;

public class PropertyInfoCollectionAssertions(IEnumerable<PropertyInfo> actualValue, AssertionChain chain)
    : GenericCollectionAssertions<PropertyInfo>(actualValue, chain)
{
    protected override string Identifier => "properties";

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
