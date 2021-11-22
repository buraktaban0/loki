using System;
using System.Collections.Generic;
using UnityEngine;

namespace Loki.Runtime.Core
{
	public class LokiScope
	{
		public Dictionary<string, object> Values = new Dictionary<string, object>();
		public object CurrentContext;

		private string GetFullyQualifiedId(string id) => $"{CurrentContext}_{id}";

		public ILokiValue<T> GetValue<T>(string id)
		{
			var fullyQualifiedId = GetFullyQualifiedId(id);

			if (!Values.TryGetValue(fullyQualifiedId, out var para))
			{
				Debug.LogException(new Exception($"Could not find any value named {id} in scope."));
				return new DefaultValue<T>();
			}

			if (!(para is ILokiValue<T> input))
			{
				Debug.LogException(new Exception(
					                   $"Value named {id} exists but it cannot be converted to {typeof(T).FullName}"));

				return new DefaultValue<T>();
			}

			return input;
		}

		public void SetValue<T>(string id, LokiValue<T> value)
		{
			var fullyQualifiedId = GetFullyQualifiedId(id);

			Values[id] = value;
		}
	}
}
