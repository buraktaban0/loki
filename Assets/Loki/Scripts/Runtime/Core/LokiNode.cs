using System;
using System.Linq;

namespace Loki.Runtime.Core
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

		public virtual LokiFlowGate[] FlowGates => Array.Empty<LokiFlowGate>();
		public virtual LokiInput[]    Inputs    => Array.Empty<LokiInput>();
		public virtual LokiOutput[]   Outputs   => Array.Empty<LokiOutput>();

		protected abstract string Name { get; }

		public abstract void Process(LokiScope scope);
	}
}
