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
        private readonly int _minLength;
        private readonly int _maxLength;
        private readonly Encoding _encoding;
        private static readonly Random Random = new Random();


        public RandomTextContentGenerator(int minLength, int maxLength) : this(minLength, maxLength, Encoding.UTF8)
        {
        }

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