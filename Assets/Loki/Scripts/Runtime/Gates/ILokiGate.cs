using Loki.Runtime.Core;

namespace Loki.Runtime.Gates
{
	public interface ILokiGate
	{
		public string Name { get; }

		public string Guid { get; set; }

		public Direction Direction { get; }

		public int Capacity { get; }
	}
}
