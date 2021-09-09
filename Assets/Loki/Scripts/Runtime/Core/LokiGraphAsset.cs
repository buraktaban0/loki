using UnityEngine;

namespace Loki.Runtime.Core
{
	[System.Serializable]
	public class LokiGraphAsset : ScriptableObject
	{
		[SerializeField]
		private LokiGraph m_Graph;

		public LokiGraph Graph => m_Graph;
	}
}
