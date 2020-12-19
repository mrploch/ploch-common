using System.Collections.Generic;

namespace Ploch.Common.Xml.Simple
{
    public static class Attributes
    {
        public static AttributesBuilder Create(string name, string value)
        {
            return new AttributesBuilder {{name, value}};
        }

        public class AttributesBuilder : Dictionary<string, string>
        {
            public new AttributesBuilder Add(string name, string value)
            {
                base.Add(name, value);
                return this;
            }
        }
    }
}