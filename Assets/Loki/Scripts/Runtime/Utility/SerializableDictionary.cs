using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace Loki.Runtime.Utility
{
    [System.Serializable]
    public class SerializableDictionary<TKey, TVal> : ISerializationCallbackReceiver
    {
        [SerializeField] private List<TKey> m_Keys;
        [SerializeField] private List<TVal> m_Values;

        private Dictionary<TKey, TVal> m_Dict = new();

        public TVal this[TKey key]
        {
            get => m_Dict[key];
            set => m_Dict[key] = value;
        }

        public bool TryGetValue(TKey key, out TVal val)
        {
            return m_Dict.TryGetValue(key, out val);
        }

        public bool Remove(TKey key)
        {
            return m_Dict.Remove(key);
        }

        public void OnBeforeSerialize()
        {
            m_Keys = m_Dict.Keys.ToList();
            m_Values = m_Dict.Values.ToList();
        }

        public void OnAfterDeserialize()
        {
            m_Dict = m_Keys.Zip(m_Values, (key, val) => (key, val)).ToDictionary(kv => kv.key, kv => kv.val);
        }
    }
}