using System;
using System.Linq;
using System.Reflection;

namespace Loki.Runtime.Core
{
	public class LokiParameter
	{
		public Direction Direction;

		public string Name;

		public Type Type;

		public static LokiParameter[] ExtractInputs(ParameterInfo[] parameters)
		{
			return parameters.Where(info => !info.IsOut && !info.IsRetval).Select(FromParameterInfo).ToArray();
		}

		public static LokiParameter[] ExtractOutputs(ParameterInfo[] parameters)
		{
			return parameters.Where(info => info.IsOut || info.IsRetval).Select(FromParameterInfo).ToArray();
		}

		public static LokiParameter FromParameterInfo(ParameterInfo info)
		{
			var direction = info.IsOut || info.IsRetval ? Direction.Output : Direction.Input;
			return new LokiParameter
			{
				Direction = direction,
				Name = info.Name,
				Type = info.ParameterType
			};
		}
	}
}
