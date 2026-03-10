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
/// <param name="filePath">The absolute or relative path to the text file to load.</param>
[AttributeUsage(AttributeTargets.Method, AllowMultiple = true)]
public sealed class TextFileLinesDataAttribute(string filePath, bool removeEmptyEntries = false) : TextFileDataAttribute(filePath)
{
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
    var lines = fileData.Split(new[] { "\r\n", "\n" }, StringSplitOptions.None);

    if (removeEmptyEntries)
    {
      lines = lines.Where(line => !string.IsNullOrWhiteSpace(line)).ToArray();
    }

    return lines.Select(line => new object?[] { line });
  }
}
