using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace Loki.Runtime.Core
{
	[System.Serializable]
	public class LokiGraph : ISerializationCallbackReceiver
	{
		[SerializeReference]
		private List<LokiNode> m_Nodes;

		public List<LokiNode> Nodes
		{
			get => m_Nodes;
			set => m_Nodes = value;
		}


		[SerializeField]
		private List<LokiConnection> m_Connections;

		public List<LokiConnection> Connections
		{
			get => m_Connections;
			set => m_Connections = value;
		}

#if UNITY_EDITOR

		[SerializeField]
		private Vector2 m_ViewPosition;

		public Vector2 ViewPosition => m_ViewPosition;

		[SerializeField]
		private List<Vector2> m_NodePositions;

		public List<Vector2> NodePositions => m_NodePositions;

#endif

		public T GetNode<T>() where T : LokiNode
		{
			return m_Nodes.FirstOrDefault(node => node is T) as T;
		}

		public T GetNode<T>(string userIdentifier) where T : LokiNode
		{
			return m_Nodes.FirstOrDefault(node => node is T &&
			                                      string.CompareOrdinal(node.UserIdentifier, userIdentifier) == 0) as T;
		}

		public void OnBeforeSerialize()
		{
		}

		public void OnAfterDeserialize()
		{
			for (var i = 0; i < m_Nodes.Count; i++)
			{
				var node = m_Nodes[i];
				node.Graph = this;
			}
		}
	}
}
