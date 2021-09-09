using System;

namespace Ploch.TestingSupport.TestData
{
    public static class RandomData
    {
        private static readonly Random Random = new Random();

        private const string DefaultAllowedCharacters = "ABCDEFGHJKLMNOPQRSTUVWXYZabcdefghijkmnopqrstuvwxyz0123456789!@$?_- ";
        public static string GenerateString(int stringLength, string allowedCharacters = DefaultAllowedCharacters)
        {
            char[] chars = new char[stringLength];

            for (int i = 0; i < stringLength; i++)
            {
                chars[i] = allowedCharacters[Random.Next(0, allowedCharacters.Length)];
            }

            return new string(chars);
        }
    }
}