using System;
using System.Linq;
using System.Reflection;
using Loki.Runtime.Gates;

namespace Loki.Runtime.Core
{
	public class LokiValueGate : ILokiGate
	{
		public string Name { get; set; }

		public string Guid { get; set; }

		public Direction Direction { get; set; }

		public int Capacity { get; set; }

		public Type Type;

		public static LokiValueGate[] ExtractInputs(ParameterInfo[] parameters)
		{
			return parameters.Where(info => !info.IsOut && !info.IsRetval).Select(FromParameterInfo).ToArray();
		}

		public static LokiValueGate[] ExtractOutputs(ParameterInfo[] parameters)
		{
			return parameters.Where(info => info.IsOut || info.IsRetval).Select(FromParameterInfo).ToArray();
		}

		public static LokiValueGate FromParameterInfo(ParameterInfo info)
		{
			var isOutput = info.IsOut || info.IsRetval;
			return new LokiValueGate
			       {
				       Direction = isOutput ? Direction.Output : Direction.Input,
				       Name = info.Name,
				       Type = info.ParameterType,
				       Capacity = isOutput ? Gates.Capacity.Multiple : Gates.Capacity.Single,
			       };
		}
	}
}
