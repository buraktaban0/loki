using System;
using Loki.Runtime.Core;
using Loki.Runtime.Gates;

namespace Loki.Runtime.Nodes
{
	public interface ILokiNode
	{
		public string Guid { get; set; }

		public string Name { get; }

		public ILokiGate[] FlowGates { get; }
		public ILokiGate[] Inputs    { get; }
		public ILokiGate[] Outputs   { get; }

		public void InitializeRuntimeData();

		public void Process(ILokiScope scope);
	}
}
