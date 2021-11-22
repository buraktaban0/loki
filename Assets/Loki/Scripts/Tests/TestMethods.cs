using Loki.Runtime.Attributes;

namespace Loki.Tests
{
	public class TestMethods
	{
		[Loki]
		public static int TestMethod1(float inputFloat, string inputString, out bool outputBoolean)
		{
			outputBoolean = true;
			return 100;
		}
	}
}
