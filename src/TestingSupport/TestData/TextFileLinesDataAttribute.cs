using System;
using System.Collections.Generic;
using System.Linq;

namespace Ploch.TestingSupport.TestData;

/// <summary>
///   Provides a data source for a data theory, with the data coming from a text file where each line represents a test case.
///   This attribute allows loading test data from text files for use with xUnit theories, treating each line as a separate test input.
/// </summary>
/// <remarks>
///   Initializes a new instance of the <see cref="TextFileLinesDataAttribute" /> class.
///   Load data from a text file as the data source for a theory, with each line being a separate test case.
/// </remarks>
/// <param name="filePath">The path to the text file to load. A fully qualified path is used as given; any
///   other form - including a path rooted at the current drive such as <c>"/data/cases.txt"</c> - is resolved
///   against the directory of the test assembly (<see cref="AppContext.BaseDirectory" />), not the process working
///   directory.</param>
/// <param name="removeEmptyEntries">
///   When <see langword="true" />, empty or whitespace-only lines are excluded from the generated test data.
/// </param>
[AttributeUsage(AttributeTargets.Method, AllowMultiple = true)]
public sealed class TextFileLinesDataAttribute(string filePath, bool removeEmptyEntries = false) : TextFileDataAttribute(filePath)
{
  private static readonly string[] LineSeparators = ["\r\n", "\n"];

  /// <summary>
  ///   Indicates whether this data attribute supports discovery-time enumeration of test cases.
  /// </summary>
  /// <returns>
  ///   Always <see langword="false" />, because the data is loaded from a file at execution time rather than during discovery.
  /// </returns>
  public override bool SupportsDiscoveryEnumeration() => false;

  /// <summary>
  ///   Processes the specified file data and returns an enumerable collection of object arrays, each containing a line
  ///   from the file.
  /// </summary>
  /// <remarks>
  ///   Empty or whitespace-only lines are excluded from the result if the removeEmptyEntries option is
  ///   enabled.
  /// </remarks>
  /// <param name="fileData">The contents of the file to process. Each line in the string is treated as a separate entry. Cannot be null.</param>
  /// <returns>
  ///   An enumerable collection of object arrays, where each array contains a single line from the file data. If the file
  ///   data is empty, the collection will be empty.
  /// </returns>
  protected override IEnumerable<object?[]> ProcessFileData(string fileData)
  {
    var lines = fileData.Split(LineSeparators, StringSplitOptions.None);

    if (removeEmptyEntries)
    {
      lines = lines.Where(line => !string.IsNullOrWhiteSpace(line)).ToArray();
    }

    return lines.Select(line => new object?[] { line });
  }
}
