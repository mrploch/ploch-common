using Ploch.Common.ArgumentChecking;
using Xunit;

namespace Ploch.TestingSupport.XUnit3.TestData;

/// <summary>
///     Provides a data source for a data theory, with the data coming from a text file where each line represents a test case.
///     This attribute allows loading test data from text files for use with xUnit theories, treating each line as a separate test input.
/// </summary>
/// <remarks>
///     Initializes a new instance of the <see cref="TextFileLinesDataAttribute" /> class.
///     Load data from a text file as the data source for a theory, with each line being a separate test case.
/// </remarks>
/// <param name="filePath">The path to the text file to load. A fully qualified path is used as given; any
///     other form - including a path rooted at the current drive such as <c>"/data/cases.txt"</c> - is resolved
///     against the directory of the test assembly (<see cref="AppContext.BaseDirectory" />), not the process working
///     directory.</param>
/// <param name="removeEmptyEntries">When <see langword="true" />, lines that are empty or whitespace are excluded from the test data.</param>
[AttributeUsage(AttributeTargets.Method, AllowMultiple = true)]
public sealed class TextFileLinesDataAttribute(string filePath, bool removeEmptyEntries = false) : TextFileDataAttribute(filePath)
{
    /// <summary>
    ///     Gets a value indicating whether the data rows can be pre-enumerated during test discovery.
    /// </summary>
    /// <returns>Always <see langword="true" />, as each line of the text file is a discoverable test case.</returns>
    public override bool SupportsDiscoveryEnumeration() => true;

    /// <summary>
    ///     Processes the raw text data from the file by splitting it into lines, each representing a separate test case.
    /// </summary>
    /// <param name="fileData">The raw content of the text file as a string.</param>
    /// <returns>
    ///     An enumerable collection of object arrays, where each array contains a single string element
    ///     representing one line from the text file. Each array corresponds to one test case execution.
    /// </returns>
    protected override IEnumerable<ITheoryDataRow> ProcessFileData(string fileData)
    {
        // Split the file data into lines and return as object arrays
        IEnumerable<string> lines = fileData.NotNull().Split([ Environment.NewLine ], StringSplitOptions.None);

        if (removeEmptyEntries)
        {
            // Drop empty and whitespace-only lines, matching the documented contract and Ploch.TestingSupport.
            lines = lines.Where(line => !string.IsNullOrWhiteSpace(line));
        }

        return lines.Select(line => new TheoryDataRow(line));
    }
}
