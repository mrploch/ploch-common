using System;
using System.Linq;
using System.Text;

namespace Ploch.Common.Randomizers;

/// <summary>
///     Provides functionality to generate random string values.
/// </summary>
public class StringRandomizer : BaseRandomizere<string>, IRangedRandomizer<string>
{
    private readonly Random _random = new();

    /// <summary>
    ///     Generates a random string value.
    /// </summary>
    /// <returns>
    ///     A randomly generated string value consisting of 8 characters selected from upper and lower case alphabets and
    ///     digits.
    /// </returns>
    public override string GetRandomValue()
    {
        const string chars = "ABCDEFGHIJKLMNOPQRSTUVWXYZabcdefghijklmnopqrstuvwxyz0123456789";

        return new string(Enumerable.Repeat(chars, 8).Select(s => s[_random.Next(s.Length)]).ToArray());
    }

    /// <summary>
    ///     Generates a random string value consisting of specified characters between given minimum and maximum character
    ///     ranges.
    /// </summary>
    /// <param name="minChar">The minimum character in the range.</param>
    /// <param name="maxChar">The maximum character in the range.</param>
    /// <returns>
    ///     A randomly generated string value of default length 8 using characters between the specified min and max
    ///     characters.
    /// </returns>
#pragma warning disable CA1725 // Parameter names should match base declaration and other partial definitions - intentional here
    public override string GetRandomValue(string minChar, string maxChar)
#pragma warning restore CA1725
    {
        return GetRandomValue(8, minChar[0], maxChar[0]);
    }

    /// <summary>
    ///     Generates a random string value consisting of specified characters between given minimum and maximum character
    ///     ranges.
    /// </summary>
    /// <param name="numberOfCharacters">The number of characters in the generated string.</param>
    /// <param name="minChar">The minimum character in the range.</param>
    /// <param name="maxChar">The maximum character in the range.</param>
    /// <returns>
    ///     A randomly generated string value of specified length using characters between the specified min and max
    ///     characters.
    /// </returns>
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
