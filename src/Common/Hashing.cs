using System;
using System.IO;
using System.Security.Cryptography;

namespace Ploch.Common
{
    public static class Hashing
    {
        public static string ToHashString(this Stream stream, HashAlgorithm algorithm)
        {
            var hashBytes = algorithm.ComputeHash(stream);
            return BitConverter.ToString(hashBytes).Replace("-", string.Empty);
        }

        public static string ToMD5HashString(this Stream stream)
        {
            var hashBytes = MD5.Create().ComputeHash(stream);
            return BitConverter.ToString(hashBytes).Replace("-", string.Empty);
        }
    }
}