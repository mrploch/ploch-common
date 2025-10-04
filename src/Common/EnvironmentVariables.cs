using System;

namespace Ploch.Common;

/// <summary>
///     Provides methods for retrieving and converting environment variable values.
/// </summary>
public static class EnvironmentVariables
{
    /// <summary>
    ///     Retrieves the value of the specified environment variable as a string.
    /// </summary>
    /// <param name="variableName">The name of the environment variable to retrieve.</param>
    /// <returns>
    ///     A string containing the value of the environment variable,
    ///     or <c>null</c> if the environment variable is not found.
    /// </returns>
    public static string? GetString(string variableName) => Environment.GetEnvironmentVariable(variableName);

    /// <summary>
    ///     Retrieves the value of the specified environment variable as a boolean.
    /// </summary>
    /// <param name="variableName">The name of the environment variable to retrieve.</param>
    /// <returns>
    ///     A nullable boolean containing the value of the environment variable if it can be successfully parsed as a boolean,
    ///     or <c>null</c> if the environment variable is not found, not set, or cannot be parsed as a boolean.
    /// </returns>
    public static bool? GetBool(string variableName) => GetString(variableName)?.ParseToBool();

    /// <summary>
    ///     Retrieves the value of the specified environment variable and converts it to an enumeration of the specified type.
    /// </summary>
    /// <typeparam name="TEnum">The enumeration type to which the environment variable value should be converted.</typeparam>
    /// <param name="variableName">The name of the environment variable to retrieve and convert.</param>
    /// <param name="ignoreCase">
    ///     A boolean value indicating whether the case of the environment variable value should be ignored during conversion. Defaults to <c>true</c>.
    /// </param>
    /// <returns>
    ///     An instance of the specified enumeration type if the environment variable value was successfully converted;
    ///     otherwise, <c>null</c> if the environment variable value is not found or cannot be converted.
    /// </returns>
    public static TEnum? GetEnumValue<TEnum>(string variableName, bool ignoreCase = true) where TEnum : struct, Enum
    {
        var value = GetString(variableName);

        return value?.SafeParseToEnum<TEnum>(ignoreCase);
    }
}
