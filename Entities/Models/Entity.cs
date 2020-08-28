using Entities.LinkModels;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Dynamic;
using System.Text;
using System.Xml;
using System.Xml.Schema;
using System.Xml.Serialization;

namespace Entities.Models
{
    public class Entity : DynamicObject, IXmlSerializable, IDictionary<string, object>
    {
        private readonly string _root = "Entity";
        private readonly IDictionary<string, object> _expando = null;

        public object this[string key]
        {
            get => _expando[key];
            set => _expando[key] = value;
        }
        public ICollection<string> Keys => _expando.Keys;
        public ICollection<object> Values => _expando.Values;
        public int Count => _expando.Count;
        public bool IsReadOnly => _expando.IsReadOnly;

        public Entity()
        {
            _expando = new ExpandoObject();
        }

        public override bool TryGetMember(GetMemberBinder binder, out object result)
        {
            if (!_expando.TryGetValue(binder.Name, out object value))
            {
                return base.TryGetMember(binder, out result);
            }

            result = value;
            return true;
        }

        public override bool TrySetMember(SetMemberBinder binder, object value)
        {
            _expando[binder.Name] = value;
            return true;
        }
        public XmlSchema GetSchema()
        {
            throw new NotImplementedException();
        }

        public void ReadXml(XmlReader reader)
        {
            reader.ReadStartElement(_root);

            while (!reader.Name.Equals(_root))
            {
                var name = reader.Name;

                reader.MoveToAttribute("type");

                var typeContent = reader.ReadContentAsString();
                var underlyingType = Type.GetType(typeContent);

                reader.MoveToContent();
                _expando[name] = reader.ReadElementContentAs(underlyingType, null);
            }
        }

        public void WriteXml(XmlWriter writer)
        {
            foreach (var key in _expando.Keys)
            {
                var value = _expando[key];
                WriteLinksToXml(key, value, writer);
            }
        }

        private void WriteLinksToXml(string key, object value, XmlWriter writer)
        {
            writer.WriteStartElement(key);

            if (value == null)
            {
                writer.WriteString(string.Empty);
            }
            else if (value.GetType() == typeof(List<Link>))
            {
                foreach (var link in value as List<Link>)
                {
                    writer.WriteStartElement(nameof(Link));
                    WriteLinksToXml(nameof(link.Href), link.Href, writer);
                    WriteLinksToXml(nameof(link.Method), link.Method, writer);
                    WriteLinksToXml(nameof(link.Rel), link.Rel, writer);
                    writer.WriteEndElement();
                }
            }
            else
            {
                writer.WriteString(value.ToString());
            }

            writer.WriteEndElement();
        }

        public void Add(string key, object value) => _expando.Add(key, value);

        public void Add(KeyValuePair<string, object> item) => _expando.Add(item);

        public void Clear() => _expando.Clear();

        public bool Contains(KeyValuePair<string, object> item) => _expando.Contains(item);

        public bool ContainsKey(string key) => _expando.ContainsKey(key);

        public void CopyTo(KeyValuePair<string, object>[] array, int arrayIndex) => _expando.CopyTo(array, arrayIndex);

        public IEnumerator<KeyValuePair<string, object>> GetEnumerator() => _expando.GetEnumerator();

        public bool Remove(string key) => _expando.Remove(key);

        public bool Remove(KeyValuePair<string, object> item) => _expando.Remove(item);

        public bool TryGetValue(string key, [MaybeNullWhen(false)] out object value) => _expando.TryGetValue(key, out value);

        IEnumerator IEnumerable.GetEnumerator() => ((IEnumerable)_expando).GetEnumerator();
    }
}
