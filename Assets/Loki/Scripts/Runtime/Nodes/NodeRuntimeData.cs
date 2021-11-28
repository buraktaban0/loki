using Loki.Runtime.Gates;

namespace Loki.Runtime.Nodes
{
	public class NodeRuntimeData
	{
		public static NodeRuntimeData FromNode(ILokiNode node)
		{
			var data = new NodeRuntimeData
			           {
				           Node = node,
				           FlowGates = node.FlowGates,
				           InputParameters = node.Inputs,
				           OutputParameters = node.Outputs,
			           };

			return data;
		}

		public ILokiNode Node;

		public ILokiGate[] FlowGates;
		public ILokiGate[] InputParameters;
		public ILokiGate[] OutputParameters;
	}
}
