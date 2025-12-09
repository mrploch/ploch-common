using System;
using System.Collections.Generic;
using System.IO;
using System.Reflection;
using System.Text.Json;
using System.Threading.Tasks;
using Xunit;
using Xunit.Sdk;
using Xunit.v3;
using JsonDocument = System.Text.Json.JsonDocument;
using JsonElement = System.Text.Json.JsonElement;
using JsonValueKind = System.Text.Json.JsonValueKind;

namespace Ploch.TestingSupport.TestData;

/// <summary>
///     Provides a data source for a data theory, with the data coming from a JSON file.
///     This attribute allows loading test data from JSON files for use with xUnit theories.
/// </summary>
/// <remarks>
///     Initializes a new instance of the <see cref="JsonFileDataAttribute" /> class.
///     Load data from a JSON file as the data source for a theory.
/// </remarks>
/// <param name="filePath">The absolute or relative path to the JSON file to load.</param>
/// <param name="propertyName">The name of the property on the JSON file that contains the data for the test.</param>
[AttributeUsage(AttributeTargets.Method, AllowMultiple = true)]
#pragma warning disable CC0023 - Mark attribute as sealed - this attribute might be a candidate for extension.
public class JsonFileDataAttribute(string filePath, string? propertyName = null) : DataAttribute
#pragma warning restore CC0023
{
  public override async ValueTask<IReadOnlyCollection<ITheoryDataRow>> GetData(MethodInfo testMethod, DisposalTracker disposalTracker)
  {
    if (testMethod == null)
    {
      throw new ArgumentNullException(nameof(testMethod));
    }

    if (string.IsNullOrWhiteSpace(filePath))
    {
      throw new ArgumentException("File path cannot be null or empty.", nameof(filePath));
    }

    var fileContent = File.ReadAllText(filePath);
    var jsonData = JsonDocument.Parse(fileContent);

    JsonElement dataElement;
    if (!string.IsNullOrEmpty(propertyName))
    {
      if (!jsonData.RootElement.TryGetProperty(propertyName!, out dataElement))
      {
        throw new ArgumentException($"Property '{propertyName}' not found in JSON file.", nameof(propertyName));
      }
    }
    else
    {
      dataElement = jsonData.RootElement;
    }

    if (dataElement.ValueKind != JsonValueKind.Array)
    {
      throw new ArgumentException("JSON data must be an array.", nameof(filePath));
    }

    var theoryDataRows = new List<ITheoryDataRow>();
    foreach (var element in dataElement.EnumerateArray())
    {
      var data = JsonSerializer.Deserialize<object[]>(element.GetRawText());

      if (data == null)
      {
        throw new InvalidOperationException("Failed to deserialize JSON data into test data.");
      }

      theoryDataRows.Add(new TheoryDataRow(data));
    }

    return theoryDataRows;
  }

  public override bool SupportsDiscoveryEnumeration() => false;
}
