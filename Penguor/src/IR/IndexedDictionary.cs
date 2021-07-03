using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics.CodeAnalysis;

namespace Penguor.Compiler.IR
{
    public class IndexedDictionary<TKey, TValue> : IDictionary<TKey, TValue>
        where TKey : notnull
        where TValue : notnull
    {
        public struct Enumerator : IEnumerator<KeyValuePair<TKey, TValue>>
        {
            private readonly IndexedDictionary<TKey, TValue> dictionary;
            private int currentIndex;

            public KeyValuePair<TKey, TValue> Current
            {
                get
                {
                    try
                    {
                        return new KeyValuePair<TKey, TValue>(dictionary.keyList[currentIndex], dictionary.values[currentIndex]);
                    }
                    catch (IndexOutOfRangeException)
                    {
                        throw new InvalidOperationException();
                    }
                }
            }

            object IEnumerator.Current => Current;

            public Enumerator(IndexedDictionary<TKey, TValue> dictionary)
            {
                this.dictionary = dictionary;
                currentIndex = -1;
            }

            public void Dispose() { }

            public bool MoveNext()
            {
                currentIndex++;
                return currentIndex < dictionary.Count;
            }

            public void Reset()
            {
                currentIndex = -1;
            }
        }

        private readonly Dictionary<TKey, int> keys;
        private readonly List<TKey> keyList;
        private readonly List<TValue> values;

        public ICollection<TKey> Keys => keys.Keys;
        public ICollection<TValue> Values => new ReadOnlyCollection<TValue>(values);

        public int Count => keys.Count;

        public bool IsReadOnly => false;

        public IndexedDictionary()
        {
            keys = new();
            keyList = new();
            values = new();
        }

        public IndexedDictionary(int capacity)
        {
            keys = new(capacity);
            keyList = new(capacity);
            values = new(capacity);
        }

        public TValue this[TKey key]
        {
            get => values[keys[key]];
            set => values[keys[key]] = value;
        }

        public TValue this[int index]
        {
            get => values[index];
            set => values[index] = value;
        }

        public TValue this[Index index]
        {
            get => values[index];
            set => values[index] = value;
        }

        private void AddKey(TKey key)
        {
            keys.Add(key, keys.Count);
            keyList.Add(key);
        }

        public void Add(TKey key, TValue value)
        {
            AddKey(key);
            values.Add(value);
        }

        public void Add(KeyValuePair<TKey, TValue> item)
        {
            AddKey(item.Key);
            values.Add(item.Value);
        }

        public void Clear()
        {
            keys.Clear();
            keyList.Clear();
            values.Clear();
        }

        public bool Contains(KeyValuePair<TKey, TValue> item) => keyList.Contains(item.Key) && this[item.Key].Equals(item.Value);

        public bool ContainsKey(TKey key) => keyList.Contains(key);

        public void CopyTo(KeyValuePair<TKey, TValue>[] array, int arrayIndex) => throw new NotImplementedException();

        public IEnumerator<KeyValuePair<TKey, TValue>> GetEnumerator() => new Enumerator(this);

        public bool Remove(TKey key)
        {
            if (!keyList.Contains(key))
                return false;
            try
            {
                values.RemoveAt(keys[key]);
                keyList.Remove(key);
                keys.Remove(key);
            }
            catch (ArgumentOutOfRangeException)
            {
                return false;
            }
            return true;
        }

        public bool Remove(KeyValuePair<TKey, TValue> item) => throw new NotImplementedException();

        public bool TryGetValue(TKey key, [MaybeNullWhen(false)] out TValue value) => throw new NotImplementedException();

        IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();
    }
}