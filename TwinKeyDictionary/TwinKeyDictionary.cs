namespace TwinKeyDictionary
{
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
    /// <summary>
    /// Represents a dictionary with two keys.
    /// Records can be looked up either by using only the primary key, or by using both the primary and secondary keys.
    /// If there are multiple records with the same primary key and no secondary key is specified, the first record with that primary key is returned.
    /// </summary>
    /// <typeparam name="TKeyPrimary">The type of the primary key.</typeparam>
    /// <typeparam name="TKeySecondary">The type of the secondary key.</typeparam>
    /// <typeparam name="TValue">The type of the values in the dictionary.</typeparam>
    public class TwinKeyDictionary<TKeyPrimary,TKeySecondary,TValue> :  
        Dictionary<Tuple<TKeyPrimary, TKeySecondary>, TValue>,
        IDictionary<TKeyPrimary, TValue>
    {
        ICollection<TKeyPrimary> IDictionary<TKeyPrimary, TValue>.Keys => Keys.Select(x => x.Item1).Distinct().ToList();

        ICollection<TValue> IDictionary<TKeyPrimary, TValue>.Values => Values;

        public bool IsReadOnly => false;

        public TValue this[TKeyPrimary primaryKey, TKeySecondary secondaryKey]
        {
            get { return base[Tuple.Create(primaryKey, secondaryKey)]; }
            set { base[Tuple.Create(primaryKey, secondaryKey)] = value; }
        }

        public TValue this[TKeyPrimary primaryKey]
        {
            get { return base[GetKeyByPrimary(primaryKey)]; }
            set { base[GetKeyByPrimary(primaryKey)] = value; }
        }

        public void Add(TKeyPrimary primaryKey, TKeySecondary secondaryKey, TValue value)
        {
            Add(Tuple.Create(primaryKey, secondaryKey), value);
        }

        public bool ContainsKey(TKeyPrimary primaryKey, TKeySecondary secondaryKey)
        {
            return ContainsKey(Tuple.Create(primaryKey, secondaryKey));
        }

        public bool ContainsKey(TKeyPrimary primaryKey)
        {
            return ContainsKey(primaryKey, out _);
        }

        private bool ContainsKey(TKeyPrimary primaryKey, out Tuple<TKeyPrimary, TKeySecondary> key)
        {
            key = GetKeyByPrimary(primaryKey);
            return key != null;
        }

        public bool TryGetValue(TKeyPrimary primaryKey, out TValue value)
        {
            value = default(TValue);
            if (ContainsKey(primaryKey, out var key))
            {
                value = this[key];
                return true;
            }
            return false;
        }

        public bool TryGetValue(TKeyPrimary primaryKey, TKeySecondary secondaryKey, out TValue value)
        {
            return TryGetValue(Tuple.Create(primaryKey, secondaryKey), out value);
        }

        private Tuple<TKeyPrimary, TKeySecondary> GetKeyByPrimary(TKeyPrimary primaryKey)
        {
            return Keys.FirstOrDefault(x => x.Item1.Equals(primaryKey));
        }

        public bool Remove(TKeyPrimary primaryKey)
        {
            bool hasKey = ContainsKey(primaryKey, out var key);
            if (!hasKey) return false;
            return Remove(key);
        }

        public bool Remove(TKeyPrimary primaryKey, TKeySecondary secondaryKey)
        {
            return Remove(Tuple.Create(primaryKey, secondaryKey));
        }

        public void Add(TKeyPrimary key, TValue value)
        {
            Add(Tuple.Create(key, default(TKeySecondary)), value);
        }

        public void Add(KeyValuePair<TKeyPrimary, TValue> item)
        {
            Add(item.Key, item.Value);
        }

        public bool Contains(KeyValuePair<TKeyPrimary, TValue> item)
        {
            bool hasKey = ContainsKey(item.Key, out var key);
            if (!hasKey) return false;
            return this.Contains(new KeyValuePair<Tuple<TKeyPrimary, TKeySecondary>, TValue>(key, item.Value));
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
            return (this as IEnumerable<KeyValuePair<Tuple<TKeyPrimary, TKeySecondary>, TValue>>)
                .Select(x => new KeyValuePair<TKeyPrimary, TValue>(x.Key.Item1, x.Value));
        }
    }
}