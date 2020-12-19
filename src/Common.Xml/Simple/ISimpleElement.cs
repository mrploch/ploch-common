using System.Collections.Generic;

namespace Ploch.Common.Xml.Simple
{
    public interface ISimpleElement : ISimpleElementCollection
    {
        IDictionary<string, string> Attributes { get; }
        string Name { get; set; }
        string Value { get; set; }
    }
}