using System;
using System.Collections.Generic;
using Loki.Runtime.Utility;
using UnityEngine;

namespace Loki.Runtime.Core
{
	public class LokiScope : ILokiScope
	{
		public MultiDictionary<string, string> ParameterDependencyMap = new MultiDictionary<string, string>();

		public Dictionary<string, object> Values = new Dictionary<string, object>();


		private List<string> GetDependencies(string id)
		{
			if (!ParameterDependencyMap.TryGetValues(id, out var values))
			{
				values = new List<string>();
			}

			return values;
		}

		public ILokiValue<T> GetValue<T>(string id)
		{
			if (!Values.TryGetValue(id, out var para))
			{
				Debug.LogException(new Exception($"Could not find any value named {id} in scope."));
				return new DefaultValue<T>();
			}

			if (!(para is ILokiValue<T> input))
			{
				Debug.LogException(new Exception(
					                   $"Value named {id} exists in scope but it cannot be converted to {typeof(T).FullName}. Actual type is {para.GetType().FullName}"));

				return new DefaultValue<T>();
			}

			return input;
		}

		public ILokiValue<object> GetValue(string id)
		{
			return GetValue<object>(id);
		}

		public void SetValue<T>(string id, ILokiValue<T> value)
		{
			Values[id] = value;

			var dependencies = GetDependencies(id);
			int count = dependencies?.Count ?? 0;
			for (var i = 0; i < count; i++)
			{
				var dependentId = dependencies[i];
				SetValue(dependentId, value);
			}
		}

		public void AddDependency(string fromId, string toId)
		{
			ParameterDependencyMap.Add(fromId, toId);
		}

		public bool HasValue(string name)
		{
			return Values.ContainsKey(name);
		}
	}
}
