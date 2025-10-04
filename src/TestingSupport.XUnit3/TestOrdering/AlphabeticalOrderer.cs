using Ploch.Common.ArgumentChecking;
using Xunit.Sdk;
using Xunit.v3;

namespace Ploch.TestingSupport.XUnit3.TestOrdering;

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
    public IReadOnlyCollection<TTestCase> OrderTestCases<TTestCase>(IReadOnlyCollection<TTestCase> testCases) where TTestCase : notnull, ITestCase
    {
        testCases.NotNull();

        var result = testCases.ToList();
        result.Sort((x, y) => StringComparer.OrdinalIgnoreCase.Compare(x.TestMethod!.MethodName, y.TestMethod!.MethodName));

        return result;
    }
}
