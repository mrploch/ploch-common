using System.Collections;
using System.Collections.Generic;
using System.Linq;

namespace Ploch.Common.Xml.Simple
{
    public class SimpleElementCollection : ISimpleElementCollection
    {
        private readonly IList<SimpleElement> _elements;

        public SimpleElementCollection() : this(new List<SimpleElement>())
        { }

        public SimpleElementCollection(IEnumerable<SimpleElement> elements)
        {
            _elements = elements != null ? new List<SimpleElement>(elements) : new List<SimpleElement>();
        }

        public IEnumerable<SimpleElement> this[string key]
        {
            get { return _elements.Where(el => el.Name == key); }
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

        public void Add(string name, Dictionary<string, string> attributes = null)
        {
            Add(name, null, attributes);
        }

        public void Add(string name, string value, Dictionary<string, string> attributes = null, List<SimpleElement> elements = null)
        {
            _elements.Add(new SimpleElement(name, value, attributes, elements));
        }

        /// <inheritdoc />
        public void Clear()
        {
            _elements.Clear();
        }

        public bool Contains(string elementName)
        {
            return _elements.Any(el => el.Name == elementName);
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
    }
}