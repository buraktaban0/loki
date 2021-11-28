using Loki.Runtime.Gates;

namespace Loki.Runtime.Core
{
	public class LokiFlowGate : ILokiGate
	{
		public string Name { get; set; }

		public string Guid { get; set; }

		public int Capacity { get; set; }

		public Direction Direction { get; set; }
	}
}
