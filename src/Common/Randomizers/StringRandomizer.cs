using System;
using System.Linq;
using System.Text;

namespace Ploch.Common.Randomizers;

/// <summary>
///     Provides functionality to generate random string values.
/// </summary>
public class StringRandomizer : IRangedRandomizer<string>
{
    private readonly Random _random = new();

    /// <summary>
    ///     Generates a random string value.
    /// </summary>
    /// <returns>
    ///     A randomly generated string value consisting of 8 characters selected from upper and lower case alphabets and
    ///     digits.
    /// </returns>
    public string GetRandomValue()
    {
        const string chars = "ABCDEFGHIJKLMNOPQRSTUVWXYZabcdefghijklmnopqrstuvwxyz0123456789";
        return new string(Enumerable.Repeat(chars, 8)
                                    .Select(s => s[_random.Next(s.Length)])
                                    .ToArray());
    }

    public string GetRandomValue(string minChar, string maxChar)
    {
        return GetRandomValue(8, minChar[0], maxChar[0]);
    }

    public string GetRandomValue(int numberOfCharacters, char minChar = '0', char maxChar = 'Z')
    {
        var stringBuilder = new StringBuilder();
        for (var i = 0; i < numberOfCharacters; i++)
        {
            stringBuilder.Append((char)_random.Next(minChar, maxChar));
        }

        return stringBuilder.ToString();
    }
}