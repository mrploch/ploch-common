using System;
using System.Collections.Generic;
using System.Linq;
using Xunit.Abstractions;
using Xunit.Sdk;

namespace Ploch.TestingSupport.TestOrdering;

/// <summary>
///     Provides alphabetical ordering of test cases for xUnit test execution.
/// </summary>
public class AlphabeticalOrderer : ITestCaseOrderer
{
    /// <summary>
    ///     Orders test cases alphabetically by test method name, using a case-insensitive comparison.
    /// </summary>
    /// <typeparam name="TTestCase">The type of test case to order.</typeparam>
    /// <param name="testCases">The test cases to be ordered.</param>
    /// <returns>A collection of test cases sorted alphabetically by method name.</returns>
    public IEnumerable<TTestCase> OrderTestCases<TTestCase>(IEnumerable<TTestCase> testCases) where TTestCase : ITestCase
    {
        var result = testCases.ToList();
        result.Sort((x, y) => StringComparer.OrdinalIgnoreCase.Compare(x.TestMethod.Method.Name, y.TestMethod.Method.Name));

        return result;
    }
}
