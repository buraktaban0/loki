using System.Linq;
using Loki.Runtime.Core;
using Loki.Runtime.Database;

namespace Loki.Runtime.Nodes
{
	[System.Serializable]
	public class MethodNode : LokiNode
	{
		public SerializedMethodInfo MethodInfo;

		protected override string Name => MethodInfo.Method.Name;

		private LokiFlowGate[] m_FlowGates;

		public override LokiFlowGate[] FlowGates
		{
			get
			{
				m_FlowGates = new[]
				{
					new LokiFlowGate {Capacity = Capacity.Multiple, Direction = Direction.Input},
					new LokiFlowGate {Capacity = Capacity.Single, Direction = Direction.Output}
				};

				return m_FlowGates;
			}
		}

		private LokiParameter[] m_InputParameters;

		public override LokiParameter[] Inputs
		{
			get
			{
				m_InputParameters = LokiParameter.ExtractInputs(MethodInfo.Method.GetParameters());
				return m_InputParameters;
			}
		}

		private LokiParameter[] m_OutputParameters;

		public override LokiParameter[] Outputs
		{
			get
			{
				m_OutputParameters = LokiParameter.ExtractOutputs(MethodInfo.Method.GetParameters());
				return m_OutputParameters;
			}
		}

		public override void Process(LokiScope scope)
		{
			
		}
	}
}
