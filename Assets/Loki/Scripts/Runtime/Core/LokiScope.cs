using System;
using System.Collections.Generic;
using UnityEngine;

namespace Loki.Runtime.Core
{
	public class LokiScope
	{
		public Dictionary<string, object> Parameters = new Dictionary<string, object>();

		public ILokiValue<T> GetInput<T>(string name)
		{
			if (!Parameters.TryGetValue(name, out var para))
			{
				Debug.LogException(new Exception($"Could not find any data named {name} in scope."));
				return new DefaultLokiValue<T>();
			}

			if (!(para is ILokiValue<T> input))
			{
				Debug.LogException(new Exception(
					                   $"Parameter named {name} exists but it cannot be converted to ILokiValue<{typeof(T).FullName}>"));

				return new DefaultLokiValue<T>();
			}

			return input;
		}
	}
}
