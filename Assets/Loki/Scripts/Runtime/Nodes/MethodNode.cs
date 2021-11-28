using System.Linq;
using Loki.Runtime.Core;
using Loki.Runtime.Database;
using Loki.Runtime.Gates;
using Loki.Runtime.Utility;
using UnityEngine;

// ReSharper disable CoVariantArrayConversion

namespace Loki.Runtime.Nodes
{
	[System.Serializable]
	public class MethodNode : ILokiNode
	{
		public SerializedMethodInfo MethodInfo;

		[SerializeField]
		private string m_Guid;

		public string Guid
		{
			get => m_Guid;
			set => m_Guid = value;
		}

		public string Name => MethodInfo.Method.Name;

		private LokiFlowGate[] m_FlowGates;

		public ILokiGate[] FlowGates => m_FlowGates;

		private LokiValueGate[] m_InputGates;

		public ILokiGate[] Inputs => m_InputGates;

		private LokiValueGate[] m_OutputGates;

		public ILokiGate[] Outputs => m_OutputGates;

		public void InitializeRuntimeData()
		{
			m_FlowGates = new[]
			              {
				              new LokiFlowGate
				              {
					              Name = string.Concat("IN_", Guid), Capacity = Capacity.Multiple,
					              Direction = Direction.Input
				              },
				              new LokiFlowGate
				              {
					              Name = string.Concat("OUT_", Guid), Capacity = Capacity.Single,
					              Direction = Direction.Output
				              }
			              };

			m_InputGates = LokiValueGate.ExtractInputs(MethodInfo.Method.GetParameters());
			m_OutputGates = LokiValueGate.ExtractOutputs(MethodInfo.Method.GetParameters());
		}

		public void Process(ILokiScope scope)
		{
			var inputValues = m_InputGates.Select(parameter => scope.GetValue(parameter.Name).Value).ToArray();
			var retVal = MethodInfo.Method.InvokeStatic(inputValues);

			if (m_OutputGates.Length > 0)
			{
				var outputGate = m_OutputGates.First();
				scope.SetValue(outputGate.Name, new LokiValue<object>(retVal));
			}
		}
	}
}
