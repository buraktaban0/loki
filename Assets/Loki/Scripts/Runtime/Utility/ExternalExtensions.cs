using System.Reflection;

namespace Loki.Runtime.Utility
{
	public static class ExternalExtensions
	{
		public static object InvokeStatic(this MethodInfo methodInfo, object[] parameters)
		{
			var retVal = methodInfo.Invoke(null, parameters);
			return retVal;
		}
	}
}
