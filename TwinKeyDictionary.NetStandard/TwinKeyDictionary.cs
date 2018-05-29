using System;
using System.Collections.Generic;
using System.Linq;

namespace TwinKeyDictionary.NetStandard
{
    /// <summary>
    /// Represents a dictionary with two keys.
    /// Records can be looked up either by using only the primary key, or by using both the primary and secondary keys.
    /// If there are multiple records with the same primary key and no secondary key is specified, the first record with that primary key is returned.
    /// </summary>
    /// <typeparam name="TKeyPrimary">The type of the primary key.</typeparam>
    /// <typeparam name="TKeySecondary">The type of the secondary key.</typeparam>
    /// <typeparam name="TValue">The type of the values in the dictionary.</typeparam>
    public class TwinKeyDictionary<TKeyPrimary,TKeySecondary,TValue> :  
        Dictionary<(TKeyPrimary Primary, TKeySecondary Secondary), TValue>,
        IDictionary<TKeyPrimary, TValue>
    {
        ICollection<TKeyPrimary> IDictionary<TKeyPrimary, TValue>.Keys => Keys.Select(x => x.Item1).Distinct().ToList();

        ICollection<TValue> IDictionary<TKeyPrimary, TValue>.Values => Values;

        public bool IsReadOnly => false;

        public TValue this[TKeyPrimary primaryKey, TKeySecondary secondaryKey]
        {
            get { return base[(primaryKey, secondaryKey)]; }
            set { base[(primaryKey, secondaryKey)] = value; }
        }

        public TValue this[TKeyPrimary primaryKey]
        {
            get { return base[GetKeyByPrimary(primaryKey)]; }
            set { base[GetKeyByPrimary(primaryKey)] = value; }
        }

        public void Add(TKeyPrimary primaryKey, TKeySecondary secondaryKey, TValue value)
        {
            Add((primaryKey, secondaryKey), value);
        }

        public bool ContainsKey(TKeyPrimary primaryKey, TKeySecondary secondaryKey)
        {
            return ContainsKey((primaryKey, secondaryKey));
        }

        public bool ContainsKey(TKeyPrimary primaryKey)
        {
            return ContainsKey(primaryKey, out _);
        }

        private bool ContainsKey(TKeyPrimary primaryKey, out (TKeyPrimary, TKeySecondary) key)
        {
            return TryGetKeyByPrimary(primaryKey, out key);
        }

        public bool TryGetValue(TKeyPrimary primaryKey, out TValue value)
        {
            value = default(TValue);
            if (ContainsKey(primaryKey, out (TKeyPrimary, TKeySecondary) key))
            {
                value = this[key];
                return true;
            }
            return false;
        }

        public bool TryGetValue(TKeyPrimary primaryKey, TKeySecondary secondaryKey, out TValue value)
        {
            return TryGetValue((primaryKey, secondaryKey), out value);
        }

        private bool TryGetKeyByPrimary(TKeyPrimary primaryKey, out (TKeyPrimary Primary, TKeySecondary Secondary) key)
        {
            List<(TKeyPrimary Primary, TKeySecondary Secondary)> keys = Keys.Where(x => x.Primary.Equals(primaryKey)).ToList();
            if (keys.Any())
            {
                key = keys.First();
                return true;
            }

            key = default;
            return false;
        }

        private (TKeyPrimary, TKeySecondary) GetKeyByPrimary(TKeyPrimary primaryKey)
        {
            return Keys.FirstOrDefault(x => x.Primary.Equals(primaryKey));
        }

        public bool Remove(TKeyPrimary primaryKey)
        {
            bool hasKey = ContainsKey(primaryKey, out var key);
            if (!hasKey) return false;
            return Remove(key);
        }

        public bool Remove(TKeyPrimary primaryKey, TKeySecondary secondaryKey)
        {
            return Remove((primaryKey, secondaryKey));
        }

        public void Add(TKeyPrimary key, TValue value)
        {
            Add((key, default(TKeySecondary)), value);
        }

        public void Add(KeyValuePair<TKeyPrimary, TValue> item)
        {
            Add(item.Key, item.Value);
        }

        public bool Contains(KeyValuePair<TKeyPrimary, TValue> item)
        {
            bool hasKey = ContainsKey(item.Key, out var key);
            if (!hasKey) return false;
            return this.Contains(new KeyValuePair<(TKeyPrimary, TKeySecondary), TValue>(key, item.Value));
        }

        public void CopyTo(KeyValuePair<TKeyPrimary, TValue>[] array, int arrayIndex)
        {
            AsBaseEnumerable().ToList().CopyTo(array, arrayIndex);
        }

        public bool Remove(KeyValuePair<TKeyPrimary, TValue> item)
        {
            return Remove(item.Key);
        }

        IEnumerator<KeyValuePair<TKeyPrimary, TValue>> IEnumerable<KeyValuePair<TKeyPrimary, TValue>>.GetEnumerator()
        {
            return AsBaseEnumerable().GetEnumerator();
        }

        private IEnumerable<KeyValuePair<TKeyPrimary, TValue>> AsBaseEnumerable()
        {
            return (this as IEnumerable<KeyValuePair<(TKeyPrimary Primary, TKeySecondary Secondary), TValue>>)
                .Select(x => new KeyValuePair<TKeyPrimary, TValue>(x.Key.Primary, x.Value));
        }
    }
}