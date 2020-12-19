using System.Collections.Generic;

namespace Ploch.Common.Xml.Simple
{
    public interface ISimpleElementCollection : IList<SimpleElement>
    {
        IEnumerable<SimpleElement> this[string key] { get; }

        void Add(string name, Dictionary<string, string> attributes = null);
        void Add(string name, string value, Dictionary<string, string> attributes = null, List<SimpleElement> elements = null);
        bool Contains(string elementName);
    }
}