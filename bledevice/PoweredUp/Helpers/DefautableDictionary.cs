using System.Collections;
using System.Collections.Generic;

namespace bledevice.PoweredUp.Helpers
{
    namespace DefaultableDictionary
    {
        public class DefaultableDictionary<TKey, TValue> : IDictionary<TKey, TValue>
            where TKey : notnull
        {
            private readonly IDictionary<TKey, TValue> _dictionary;
            private readonly TValue _defaultValue;

            public DefaultableDictionary(IDictionary<TKey, TValue> dictionary, TValue defaultValue)
            {
                this._dictionary = dictionary;
                this._defaultValue = defaultValue;
            }

            public IEnumerator<KeyValuePair<TKey, TValue>> GetEnumerator()
            {
                return _dictionary.GetEnumerator();
            }

            IEnumerator IEnumerable.GetEnumerator()
            {
                return GetEnumerator();
            }

            public void Add(KeyValuePair<TKey, TValue> item)
            {
                _dictionary.Add(item);
            }

            public void Clear()
            {
                _dictionary.Clear();
            }

            public bool Contains(KeyValuePair<TKey, TValue> item)
            {
                return _dictionary.Contains(item);
            }

            public void CopyTo(KeyValuePair<TKey, TValue>[] array, int arrayIndex)
            {
                _dictionary.CopyTo(array, arrayIndex);
            }

            public bool Remove(KeyValuePair<TKey, TValue> item)
            {
                return _dictionary.Remove(item);
            }

            public int Count
            {
                get { return _dictionary.Count; }
            }

            public bool IsReadOnly
            {
                get { return _dictionary.IsReadOnly; }
            }

            public bool ContainsKey(TKey key)
            {
                return _dictionary.ContainsKey(key);
            }

            public void Add(TKey key, TValue value)
            {
                _dictionary.Add(key, value);
            }

            public bool Remove(TKey key)
            {
                return _dictionary.Remove(key);
            }

            public bool TryGetValue(TKey key, out TValue value)
            {
                if (!_dictionary.TryGetValue(key, out value))
                {
                    value = _defaultValue;
                }

                return true;
            }

            public TValue this[TKey key]
            {
                get
                {
                    try
                    {
                        return _dictionary[key];
                    }
                    catch (KeyNotFoundException)
                    {
                        return _defaultValue;
                    }
                }

                set { _dictionary[key] = value; }
            }

            public ICollection<TKey> Keys
            {
                get { return _dictionary.Keys; }
            }

            public ICollection<TValue> Values
            {
                get
                {
                    var values = new List<TValue>(_dictionary.Values)
                    {
                        _defaultValue
                    };
                    return values;
                }
            }
        }

        public static class DefaultableDictionaryExtensions
        {
            public static IDictionary<TKey, TValue> WithDefaultValue<TValue, TKey>(
                this IDictionary<TKey, TValue> dictionary, TValue defaultValue)
                where TKey : notnull
            {
                return new DefaultableDictionary<TKey, TValue>(dictionary, defaultValue);
            }
        }
    }
}