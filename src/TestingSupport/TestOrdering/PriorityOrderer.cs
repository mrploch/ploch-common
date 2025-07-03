using System;
using System.Collections.Generic;
using System.Linq;
using Xunit.Abstractions;
using Xunit.Sdk;

namespace Ploch.TestingSupport.TestOrdering;

/// <summary>
///     Provides ordering functionality for xUnit test cases based on priority attributes.
///     Implements the xUnit ITestCaseOrderer interface to enable custom test execution order.
/// </summary>
public class PriorityOrderer : ITestCaseOrderer
{
    /// <summary>
    ///     Orders test cases based on their priority attribute values and then alphabetically by name.
    ///     Test cases with the same priority are sorted alphabetically by their method names.
    ///     Test cases without a priority attribute are assigned a default priority of 0.
    /// </summary>
    /// <typeparam name="TTestCase">The type of test case to order.</typeparam>
    /// <param name="testCases">The enumerable collection of test cases to be ordered.</param>
    /// <returns>An ordered enumerable of test cases, first by priority (ascending) and then alphabetically by name.</returns>
    public IEnumerable<TTestCase> OrderTestCases<TTestCase>(IEnumerable<TTestCase> testCases) where TTestCase : ITestCase
    {
        var sortedMethods = new SortedDictionary<int, List<TTestCase>>();

        foreach (var testCase in testCases)
        {
            var priority = 0;

            foreach (var attr in testCase.TestMethod.Method.GetCustomAttributes(typeof(TestPriorityAttribute).AssemblyQualifiedName))
            {
                priority = attr.GetNamedArgument<int>(nameof(TestPriorityAttribute.Priority));
            }

            GetOrCreate(sortedMethods, priority).Add(testCase);
        }

        foreach (var list in sortedMethods.Keys.Select(priority => sortedMethods[priority]))
        {
            list.Sort((x, y) => StringComparer.OrdinalIgnoreCase.Compare(x.TestMethod.Method.Name, y.TestMethod.Method.Name));
            foreach (var testCase in list)
            {
                yield return testCase;
            }
        }
    }

    /// <summary>
    ///     Gets an existing value from a dictionary by key or creates a new instance if the key doesn't exist.
    /// </summary>
    /// <typeparam name="TKey">The type of the dictionary key.</typeparam>
    /// <typeparam name="TValue">The type of the dictionary value, which must have a parameterless constructor.</typeparam>
    /// <param name="dictionary">The dictionary to retrieve from or add to.</param>
    /// <param name="key">The key to look up in the dictionary.</param>
    /// <returns>The existing value for the specified key, or a new instance if the key wasn't found.</returns>
    private static TValue GetOrCreate<TKey, TValue>(IDictionary<TKey, TValue> dictionary, TKey key) where TValue : new()
    {
        if (dictionary.TryGetValue(key, out var result))
        {
            return result;
        }

        result = new TValue();
        dictionary[key] = result;

        return result;
    }
}
