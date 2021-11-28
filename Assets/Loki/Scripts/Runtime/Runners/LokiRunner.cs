using System;
using System.Collections.Generic;
using System.Linq;
using Loki.Runtime.Nodes;
using UnityEngine;

namespace Loki.Runtime.Core
{
	public class LokiRunner : ILokiRunner
	{
		public static LokiRunner FromGraph(LokiBehaviourGraph graph)
		{
			var runner = new LokiRunner();
			runner.Prepare(graph);
			return runner;
		}


		public LokiBehaviourGraph BehaviourGraph { get; set; }

		private LokiScope m_Scope;

		private List<ILokiNode> m_OrderedNodes;

		private void Prepare(LokiBehaviourGraph graph)
		{
			BehaviourGraph = graph;

			m_Scope = new LokiScope();

			PrepareNodeExecutionOrders();
			PrepareScope();
		}

		private void PrepareScope()
		{
			foreach (var conn in BehaviourGraph.Connections)
			{
				m_Scope.AddDependency(conn.FromGuid, conn.ToGuid);
			}
		}

		private void PrepareNodeExecutionOrders()
		{
			m_OrderedNodes = new List<ILokiNode>(BehaviourGraph.Nodes);
			try
			{
				m_OrderedNodes.Sort(CompareNodesByDependency);
			}
			catch (Exception e)
			{
				Debug.LogException(e);
			}
		}

		private int CompareNodesByDependency(ILokiNode node1, ILokiNode node2)
		{
			if (BehaviourGraph.Connections.Any(conn => ConnectionConsistsOf(conn, node1, node2)))
			{
				return -1;
			}

			return 0;
		}

		private static bool ConnectionConsistsOf(LokiConnection connection, ILokiNode node1, ILokiNode node2)
		{
			return string.CompareOrdinal(connection.FromGuid, node1.Guid) == 0 &&
			       string.CompareOrdinal(connection.ToGuid, node2.Guid) == 0;
		}

		public void Run()
		{
			for (var i = 0; i < m_OrderedNodes.Count; i++)
			{
				var node = m_OrderedNodes[i];
				node.Process(m_Scope);
			}
		}

		public T GetData<T>(string name)
		{
			return m_Scope.GetValue<T>(name).Value;
		}

		public object GetData(string name)
		{
			return m_Scope.GetValue(name).Value;
		}

		public bool HasData(string name)
		{
			return m_Scope.HasValue(name);
		}

		public void SetData<T>(string name, T value)
		{
			m_Scope.SetValue(name, new LokiValue<T>(value));
		}
	}
}
