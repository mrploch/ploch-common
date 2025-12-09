using System;
using System.Collections.Generic;
using System.Linq;
using Xunit.Sdk;
using Xunit.v3;

namespace Ploch.TestingSupport.TestOrdering;

/// <summary>
///   Provides alphabetical ordering of test cases for xUnit test execution.
/// </summary>
public class AlphabeticalOrderer : ITestCaseOrderer
{
  /// <summary>
  ///   Orders test cases alphabetically by test method name, using a case-insensitive comparison.
  /// </summary>
  /// <typeparam name="TTestCase">The type of test case to order.</typeparam>
  /// <param name="testCases">The test cases to be ordered.</param>
  /// <returns>A collection of test cases sorted alphabetically by method name.</returns>
  public IReadOnlyCollection<TTestCase> OrderTestCases<TTestCase>(IReadOnlyCollection<TTestCase> testCases) where TTestCase : notnull, ITestCase
  {
    var result = testCases.ToList();
    result.Sort((x, y) =>
                  StringComparer.OrdinalIgnoreCase.Compare(x.TestMethod?.MethodName ??
                                                           throw new InvalidOperationException("TestMethod MethodName cannot be null"),
                                                           y.TestMethod?.MethodName ??
                                                           throw new InvalidOperationException("TestMethod MethodName cannot be null")));

    return result;
  }
}
