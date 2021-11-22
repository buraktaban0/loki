using System;
using Loki.Runtime.Core;

namespace Loki.Runtime.Nodes
{
	[System.Serializable]
	public abstract class LokiNode
	{
		public static class Capacity
		{
			public const int None = 0;
			public const int Single = 1;
			public const int Multiple = 64;
		}

		public LokiGraph Graph { get; internal set; }

		public string Guid = System.Guid.NewGuid().ToString();

		public string UserIdentifier;


		public virtual LokiFlowGate[]  FlowGates => Array.Empty<LokiFlowGate>();
		public virtual LokiParameter[] Inputs    => Array.Empty<LokiParameter>();
		public virtual LokiParameter[] Outputs   => Array.Empty<LokiParameter>();

		protected abstract string Name { get; }

		public abstract void Process(LokiScope scope);
	}
}
