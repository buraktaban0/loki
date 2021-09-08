using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Loki.Runtime
{
	[System.Serializable]
	public class LokiGraph
	{
		public UnityEngine.Object owner;

		public string GetName()
		{
			return $"Graph - {owner.name}";
		}
	}
}
