using System;
using System.Collections.Generic;
using UnityEngine;

namespace HikanyanLaboratory.System
{
    [Serializable]
    public class SerializableDictionary<TKey, TValue, TPair> where TPair : Pair<TKey, TValue>
    {
        [SerializeField] private List<TPair> _pairs;

        public List<TPair> Pairs => _pairs;

        public SerializableDictionary()
        {
            _pairs = new List<TPair>();
        }

        public void Add(TKey key, TValue value)
        {
            _pairs.Add((TPair)Activator.CreateInstance(typeof(TPair), key, value));
        }

        public bool TryGetValue(TKey key, out TValue value)
        {
            foreach (var pair in _pairs)
            {
                if (EqualityComparer<TKey>.Default.Equals(pair.Key, key))
                {
                    value = pair.Value;
                    return true;
                }
            }

            value = default;
            return false;
        }

        public List<TKey> Keys()
        {
            List<TKey> keys = new List<TKey>();
            foreach (var pair in _pairs)
            {
                keys.Add(pair.Key);
            }

            return keys;
        }

        public List<TValue> Values()
        {
            List<TValue> values = new List<TValue>();
            foreach (var pair in _pairs)
            {
                values.Add(pair.Value);
            }

            return values;
        }
    }
}