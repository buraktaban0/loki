using System.Reflection;

namespace Loki.Runtime.Core
{
	internal static class LokiExtensions
	{
		internal static void SetInput(this LokiNode node, string fieldName, object value)
		{
			node.GetType().GetField(fieldName, BindingFlags.Public)?.SetValue(node, value);
		}

		internal static object GetOutput(this LokiNode node, string fieldName)
		{
			return node.GetType().GetField(fieldName).GetValue(node);
		}
	}
}
