using System;
using System.Collections.Generic;
using System.IO;
using System.Reflection;
using System.Threading.Tasks;
using Ploch.Common.ArgumentChecking;
using Xunit;
using Xunit.Sdk;
using Xunit.v3;

namespace Ploch.TestingSupport.TestData;

/// <summary>
///   Base class for attributes that provide data for a data theory from a text file.
///   This abstract class handles the common file loading operations while delegating
///   the specific data processing to derived classes.
/// </summary>
/// <param name="filePath">The absolute or relative path to the text file to load.</param>
[AttributeUsage(AttributeTargets.Method, AllowMultiple = true)]
public abstract class TextFileDataAttribute(string filePath) : DataAttribute
{
  /// <summary>
  ///   Retrieves the test data for a data theory from a text file.
  /// </summary>
  /// <param name="testMethod">
  ///   The <see cref="MethodInfo" /> representing the test method for which the data is being retrieved.
  /// </param>
  /// <param name="disposalTracker">
  ///   A <see cref="DisposalTracker" /> instance to manage the disposal of resources used during the test.
  /// </param>
  /// <returns>
  ///   A task that represents the asynchronous operation. The task result contains a collection of
  ///   <see cref="ITheoryDataRow" /> objects representing the test data rows.
  /// </returns>
  /// <exception cref="ArgumentNullException">
  ///   Thrown when <paramref name="testMethod" /> is <c>null</c>.
  /// </exception>
  /// <exception cref="ArgumentException">
  ///   Thrown when the file specified by the <c>filePath</c> parameter does not exist.
  /// </exception>
  /// <remarks>
  ///   This method reads the content of the specified text file, processes the data using the
  ///   <see cref="ProcessFileData" /> method, and converts the processed data into a collection of
  ///   <see cref="ITheoryDataRow" /> objects.
  /// </remarks>
  public override async ValueTask<IReadOnlyCollection<ITheoryDataRow>> GetData(MethodInfo testMethod, DisposalTracker disposalTracker)
  {
    if (testMethod == null)
    {
      throw new ArgumentNullException(nameof(testMethod));
    }

    var path = Path.GetFullPath(filePath.NotNullOrEmpty(nameof(filePath)));
    if (!File.Exists(path))
    {
      throw new ArgumentException($"Could not find file at path: {path}");
    }

    var fileData = File.ReadAllText(path);
    var processedData = ProcessFileData(fileData);

    var theoryDataRows = new List<ITheoryDataRow>();
    foreach (var data in processedData)
    {
      theoryDataRows.Add(new TheoryDataRow(data));
    }

    return theoryDataRows;
  }

  /// <summary>
  ///   Processes the raw text data from the file and converts it into test data.
  /// </summary>
  /// <param name="fileData">The raw content of the file as a string.</param>
  /// <returns>
  ///   Enumerable of object arrays, where each array represents a set of parameters
  ///   for a test method invocation.
  /// </returns>
  protected abstract IEnumerable<object?[]> ProcessFileData(string fileData);
}
