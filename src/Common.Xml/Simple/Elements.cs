using System.Collections.Generic;

namespace Ploch.Common.Xml.Simple
{
    public static class Elements
    {
        public static ElementsCollectionBuilder Create()
        {
            return new ElementsCollectionBuilder();
        }

        public class ElementsCollectionBuilder : List<SimpleElement>
        {
            public ElementsCollectionBuilder Add(string elementName, IDictionary<string, string> attributes = null, IList<SimpleElement> elements = null)
            {
                base.Add(new SimpleElement(elementName, attributes, elements));
                return this;
            }

            public ElementsCollectionBuilder Add(string elementName,
                string elementValue,
                IDictionary<string, string> attributes = null,
                IList<SimpleElement> elements = null)
            {
                base.Add(new SimpleElement(elementName, elementValue, attributes, elements));
                return this;
            }
        }
    }
}