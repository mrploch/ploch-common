using System;

namespace Ploch.TestingSupport.TestData
{
    public static class RandomData
    {
        private const string DefaultAllowedCharacters = "ABCDEFGHJKLMNOPQRSTUVWXYZabcdefghijkmnopqrstuvwxyz0123456789!@$?_- ";
        private static readonly Random Random = new();

        public static string GenerateString(int stringLength, string allowedCharacters = DefaultAllowedCharacters)
        {
            var chars = new char[stringLength];

            for (var i = 0; i < stringLength; i++)
            {
                chars[i] = allowedCharacters[Random.Next(0, allowedCharacters.Length)];
            }

            return new string(chars);
        }
    }
}