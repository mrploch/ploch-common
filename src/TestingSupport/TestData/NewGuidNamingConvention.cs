using System;

namespace Ploch.TestingSupport.TestData
{
    public class NewGuidNamingConvention : IFileNamingConvention
    {
        private readonly string _prefix;
        private readonly string _suffix;
        private readonly string _extension;

        public NewGuidNamingConvention(string prefix, string suffix, string extension)
        {
            _prefix = prefix;
            _suffix = suffix;
            if (extension != null && !extension.StartsWith("."))
            {
                extension = "." + extension;
            }
            _extension = extension;
        }

        public string GetName(int number)
        {
            return $"{_prefix}{Guid.NewGuid()}{_suffix}{_extension}";
        }
    }
}