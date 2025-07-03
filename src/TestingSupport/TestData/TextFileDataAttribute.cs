using System;
using System.Collections.Generic;
using System.IO;
using System.Reflection;
using Ploch.Common.ArgumentChecking;
using Xunit.Sdk;

namespace Ploch.TestingSupport.TestData;

/// <summary>
///     Base class for attributes that provide data for a data theory from a text file.
///     This abstract class handles the common file loading operations while delegating
///     the specific data processing to derived classes.
/// </summary>
/// <param name="filePath">The absolute or relative path to the text file to load.</param>
public abstract class TextFileDataAttribute(string filePath) : DataAttribute
{
    /// <summary>
    ///     Returns the data to be used to test the theory by loading content from the specified text file.
    /// </summary>
    /// <param name="testMethod">The method that is being tested.</param>
    /// <returns>
    ///     Enumerable of object arrays, where each array contains the parameter values for the test method
    ///     as processed from the text file.
    /// </returns>
    /// <exception cref="ArgumentNullException">Thrown when <paramref name="testMethod" /> is null.</exception>
    /// <exception cref="ArgumentException">Thrown when the file at the specified path does not exist.</exception>
    public override IEnumerable<object?[]> GetData(MethodInfo testMethod)
    {
        if (testMethod == null)
        {
            throw new ArgumentNullException(nameof(testMethod));
        }

        // Get the absolute path to the JSON file
        var path = Path.GetFullPath(filePath.NotNullOrEmpty(nameof(filePath)));
        if (!File.Exists(path))
        {
            throw new ArgumentException($"Could not find file at path: {path}");
        }

        // Load the file
        var fileData = File.ReadAllText(filePath);

        return ProcessFileData(fileData);
    }

    /// <summary>
    ///     Processes the raw text data from the file and converts it into test data.
    /// </summary>
    /// <param name="fileData">The raw content of the file as a string.</param>
    /// <returns>
    ///     Enumerable of object arrays, where each array represents a set of parameters
    ///     for a test method invocation.
    /// </returns>
    protected abstract IEnumerable<object?[]> ProcessFileData(string fileData);
}
