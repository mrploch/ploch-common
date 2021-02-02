using System.Collections;
using System.Collections.Generic;
using System.Linq;

namespace Ploch.Common.Xml.Simple
{
    /// <summary>
    ///     <para>Simplified representation of an XML Element.</para>
    /// </summary>
    public class SimpleElement : ISimpleElement
    {
        private readonly IList<SimpleElement> _elements;

        public SimpleElement(string name, IDictionary<string, string> attributes = null, IList<SimpleElement> elements = null) : this(name, null, attributes,
            elements)
        { }

        public SimpleElement(string name, string value, IDictionary<string, string> attributes = null, IList<SimpleElement> elements = null)
        {
            Name = name;
            Value = value;
            Attributes = attributes != null ? new Dictionary<string, string>(attributes) : new Dictionary<string, string>();
            _elements = elements != null ? new List<SimpleElement>(elements) : new List<SimpleElement>();
        }

        /// <inheritdoc />
        public IEnumerator<SimpleElement> GetEnumerator()
        {
            return _elements.GetEnumerator();
        }

        /// <inheritdoc />
        IEnumerator IEnumerable.GetEnumerator()
        {
            return ((IEnumerable) _elements).GetEnumerator();
        }

        /// <inheritdoc />
        public void Add(SimpleElement item)
        {
            _elements.Add(item);
        }

        /// <inheritdoc />
        public void Clear()
        {
            _elements.Clear();
        }

        /// <inheritdoc />
        public bool Contains(SimpleElement item)
        {
            return _elements.Contains(item);
        }

        /// <inheritdoc />
        public void CopyTo(SimpleElement[] array, int arrayIndex)
        {
            _elements.CopyTo(array, arrayIndex);
        }

        /// <inheritdoc />
        public bool Remove(SimpleElement item)
        {
            return _elements.Remove(item);
        }

        /// <inheritdoc />
        public int Count => _elements.Count;

        /// <inheritdoc />
        public bool IsReadOnly => _elements.IsReadOnly;

        /// <inheritdoc />
        public int IndexOf(SimpleElement item)
        {
            return _elements.IndexOf(item);
        }

        /// <inheritdoc />
        public void Insert(int index, SimpleElement item)
        {
            _elements.Insert(index, item);
        }

        /// <inheritdoc />
        public void RemoveAt(int index)
        {
            _elements.RemoveAt(index);
        }

        /// <inheritdoc />
        public SimpleElement this[int index]
        {
            get => _elements[index];
            set => _elements[index] = value;
        }

        public string Name { get; set; }
        public string Value { get; set; }

        public IEnumerable<SimpleElement> this[string key]
        {
            get { return _elements.Where(el => el.Name == key); }
        }

        public IDictionary<string, string> Attributes { get; }

        public void Add(string name, Dictionary<string, string> attributes = null)
        {
            Add(name, null, attributes);
        }

        public void Add(string name, string value, Dictionary<string, string> attributes = null, List<SimpleElement> elements = null)
        {
            _elements.Add(new SimpleElement(name, value, attributes, elements));
        }

        public bool Contains(string elementName)
        {
            return _elements.Any(el => el.Name == elementName);
        }
    }
}