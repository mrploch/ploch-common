using System;
using System.Text;

namespace Ploch.TestingSupport.TestData
{
    public interface IContentGenerator
    {
        byte[] Generate();
    }

    public class RandomTextContentGenerator : IContentGenerator
    {
        private static readonly Random Random = new();
        private readonly Encoding _encoding;
        private readonly int _maxLength;
        private readonly int _minLength;

        public RandomTextContentGenerator(int minLength, int maxLength) : this(minLength, maxLength, Encoding.UTF8)
        { }

        public RandomTextContentGenerator(int minLength, int maxLength, Encoding encoding)
        {
            _minLength = minLength;
            _maxLength = maxLength;
            _encoding = encoding;
        }

        public byte[] Generate()
        {
            var str = RandomData.GenerateString(Random.Next(_minLength, _maxLength));

            return _encoding.GetBytes(str);
        }
    }
}