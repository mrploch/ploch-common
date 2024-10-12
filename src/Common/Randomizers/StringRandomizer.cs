using System;
using System.Linq;

namespace Ploch.Common.Randomizers;

/// <summary>
/// Provides functionality to generate random string values.
/// </summary>
/// <remarks>
/// This class implements the <see cref="IRandomizer{String}"/> interface to generate random strings.
/// </remarks>
public class StringRandomizer : IRandomizer<string>
{
    private static readonly Random Random = new();

    /// <summary>
    /// Generates a random string value.
    /// </summary>
    /// <returns>A randomly generated string value.</returns>
    /// <remarks>
    /// This method uses the <see cref="System.Random"/> class to generate a string consisting of 
    /// 8 characters chosen from uppercase letters, lowercase letters, and digits.
    /// </remarks>
    public string GetValue()
    {
        const string chars = "ABCDEFGHIJKLMNOPQRSTUVWXYZabcdefghijklmnopqrstuvwxyz0123456789";
        return new string(Enumerable.Repeat(chars, 8)
                                    .Select(s => s[Random.Next(s.Length)])
                                    .ToArray());
    }
}