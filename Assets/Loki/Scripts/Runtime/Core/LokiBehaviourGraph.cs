using System.Collections.Generic;
using System.Linq;
using Loki.Runtime.Nodes;
using UnityEngine;

namespace Loki.Runtime.Core
{
	[CreateAssetMenu(menuName = "Loki/Behaviour Graph")]
	public class LokiBehaviourGraph : ScriptableObject, ILokiGraph, ISerializationCallbackReceiver
	{
		[SerializeReference]
		private List<ILokiNode> m_Nodes;

		public List<ILokiNode> Nodes
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

		private Dictionary<string, ILokiNode> m_NodesByGuids;

#if UNITY_EDITOR

		[SerializeField]
		private Vector2 m_ViewPosition;

		public Vector2 ViewPosition => m_ViewPosition;

		[SerializeField]
		private List<Vector2> m_NodePositions;

		public List<Vector2> NodePositions => m_NodePositions;

#endif

		public void OnBeforeSerialize()
		{
		}

		public void OnAfterDeserialize()
		{
			m_NodesByGuids = m_Nodes.ToDictionary(node => node.Guid);

			for (var i = 0; i < m_Connections.Count; i++)
			{
				var con = m_Connections[i];
				var fromNode = m_NodesByGuids[con.FromGuid];
				var toNode = m_NodesByGuids[con.ToGuid];
			}
		}
	}
}
