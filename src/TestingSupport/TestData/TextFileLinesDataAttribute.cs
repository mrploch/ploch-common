using System;
using System.Collections.Generic;
using System.Linq;
using Ploch.Common;

namespace Ploch.TestingSupport.TestData;

/// <summary>
///     Provides a data source for a data theory, with the data coming from a text file where each line represents a test case.
///     This attribute allows loading test data from text files for use with xUnit theories, treating each line as a separate test input.
/// </summary>
/// <remarks>
///     Initializes a new instance of the <see cref="TextFileLinesDataAttribute" /> class.
///     Load data from a text file as the data source for a theory, with each line being a separate test case.
/// </remarks>
/// <param name="filePath">The absolute or relative path to the text file to load.</param>
public class TextFileLinesDataAttribute(string filePath, bool removeEmptyEntries = false) : TextFileDataAttribute(filePath)
{
    /// <summary>
    ///     Processes the raw text data from the file by splitting it into lines, each representing a separate test case.
    /// </summary>
    /// <param name="fileData">The raw content of the text file as a string.</param>
    /// <returns>
    ///     An enumerable collection of object arrays, where each array contains a single string element
    ///     representing one line from the text file. Each array corresponds to one test case execution.
    /// </returns>
    protected override IEnumerable<object?[]> ProcessFileData(string fileData)
    {
        // Split the file data into lines and return as object arrays
        IEnumerable<string> lines = fileData.Split([ Environment.NewLine ], StringSplitOptions.None);

        if (removeEmptyEntries)
        {
            // Remove any empty lines from the collection
            lines = lines.Where(line => line.IsNotNullOrEmpty());
        }

        return lines.Select(line => new object?[] { line });
    }
}
