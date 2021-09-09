using System.Collections.Generic;

namespace Ploch.Common.Tests.Collections
{
    public static class DictionaryBuilder
    {
        public static IDictionary<TKey, TValue> Create<TKey, TValue>(TKey key, TValue value)
        {
            return new Dictionary<TKey, TValue> {{key, value}};
        }
    }
}