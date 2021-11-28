using System;
using System.Collections.Generic;

namespace Loki.Runtime.Utility
{
	public class MultiDictionary<TKey, TVal>
	{
		private Dictionary<TKey, List<TVal>> m_Dictionary = new Dictionary<TKey, List<TVal>>();


		private List<TVal> GetValues(TKey key, bool autoCreate = true)
		{
			if (!m_Dictionary.TryGetValue(key, out var values))
			{
				if (!autoCreate)
				{
					throw new KeyNotFoundException(
						$"Values collection with key {key} was not found in multi dictionary.");
				}

				values = new List<TVal>();
				m_Dictionary[key] = values;
			}

			return values;
		}

		public void Add(TKey key, TVal val)
		{
			var values = GetValues(key);
			values.Add(val);
		}

		public bool RemovePair(TKey key, TVal val)
		{
			var values = GetValues(key);
			var wasRemoved = values.Remove(val);
			if (values.Count < 1)
			{
				m_Dictionary.Remove(key);
			}

			return wasRemoved;
		}

		public int RemoveAll(TKey key)
		{
			var values = GetValues(key);
			int count = values.Count;

			m_Dictionary.Remove(key);
			return count;
		}

		public bool ContainsKey(TKey key)
		{
			return m_Dictionary.ContainsKey(key);
		}

		public bool TryGetValues(TKey key, out List<TVal> values)
		{
			return m_Dictionary.TryGetValue(key, out values);
		}
	}
}
