
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

		protected abstract int FlowInputCapacity  { get; }
		protected abstract int FlowOutputCapacity { get; }

		protected bool HasFlowInput  => FlowInputCapacity >= Capacity.Single;
		protected bool HasFlowOutput => FlowOutputCapacity >= Capacity.Single;

		protected abstract string Name { get; }

		public abstract void Evaluate();
	}
}
