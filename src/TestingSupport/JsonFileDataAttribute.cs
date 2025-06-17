using System;
using System.Collections.Generic;
using System.IO;
using System.Reflection;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using Ploch.Common.ArgumentChecking;
using Xunit.Sdk;

namespace Ploch.TestingSupport;

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
    /// <summary>
    ///     Returns the data to be used to test the theory.
    /// </summary>
    /// <param name="testMethod">The method that is being tested.</param>
    /// <returns>
    ///     Enumerable of object arrays, where each array contains the parameter values for the test method.
    /// </returns>
    /// <exception cref="ArgumentNullException">Thrown when <paramref name="testMethod" /> is null.</exception>
    /// <exception cref="ArgumentException">
    ///     Thrown when the file at <see cref="filePath" /> does not exist or when the specified property is not found
    ///     in the JSON file.
    /// </exception>
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

        var jsonData = string.IsNullOrEmpty(propertyName) ? GetRootJsonData(fileData, path) : GetPropertyJsonData(fileData, propertyName!, path);

        return CastParamTypes(jsonData, testMethod);
    }

    private static List<object?[]> GetRootJsonData(string fileData, string filePath) => JsonConvert.DeserializeObject<List<object?[]>>(fileData) ??
                                                                                        throw new
                                                                                            InvalidOperationException($"Failed to deserialize JSON data from file: {filePath}");

    private static List<object?[]> GetPropertyJsonData(string fileData, string propertyName, string filePath)
    {
        // Parse the JSON data
        var allData = JObject.Parse(fileData);
        var data = allData[propertyName] ?? throw new ArgumentException($"Property '{propertyName}' not found in JSON file: {filePath}");

        // Deserialize the JSON data into a list of object arrays
        return data.ToObject<List<object?[]>>() ??
               throw new InvalidOperationException($"Failed to deserialize JSON data from property '{propertyName}' in file: {filePath}");
    }

    /// <summary>
    ///     Casts the objects read from the JSON file to match the types of the test method parameters.
    /// </summary>
    /// <param name="jsonData">Array of objects read from the JSON file.</param>
    /// <param name="testMethod">The method that is being tested.</param>
    /// <returns>Enumerable of object arrays with values cast to the appropriate parameter types.</returns>
#pragma warning disable CA1859 // Use concrete types when possible for improved performance - this is a signature of DataAttribute.
    private static IEnumerable<object?[]> CastParamTypes(List<object?[]> jsonData, MethodBase testMethod)
#pragma warning restore CA1859
    {
        var result = new List<object?[]>();

        // Get the parameters of the current test method
        var parameters = testMethod.GetParameters();

        // Foreach tuple of parameters in the JSON data
        foreach (var paramsTuple in jsonData)
        {
            var paramValues = new object?[parameters.Length];

            // Foreach parameter in the method
            for (var i = 0; i < parameters.Length; i++)
            {
                // Cast the value in the JSON data to match a parameter type
                paramValues[i] = CastParamValue(paramsTuple[i], parameters[i].ParameterType);
            }

            result.Add(paramValues);
        }

        return result;
    }

    /// <summary>
    ///     Casts an object from JSON data to the specified target type.
    /// </summary>
    /// <param name="value">The value to be cast.</param>
    /// <param name="type">The target type to cast the value to.</param>
    /// <returns>The value cast to the specified type, or the original value if no casting is needed.</returns>
    private static object? CastParamValue(object? value, Type type)
    {
        if (value is null)
        {
            return null;
        }

        // Cast objects
        if (value is JObject jObjectValue)
        {
            return jObjectValue.ToObject(type);
        }

        // Cast arrays
        if (value is JArray jArrayValue)
        {
            return jArrayValue.ToObject(type);
        }

        // No cast for value types
        return value;
    }
}
