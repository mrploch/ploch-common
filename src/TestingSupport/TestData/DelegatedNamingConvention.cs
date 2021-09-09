using System;

namespace Ploch.TestingSupport.TestData
{
    public class DelegatedNamingConvention : IFileNamingConvention
    {
        private readonly Func<int, string> _nameFunc;

        public DelegatedNamingConvention(Func<int, string> nameFunc)
        {
            _nameFunc = nameFunc;
        }

        public string GetName(int number)
        {
            return _nameFunc(number);
        }
    }
}