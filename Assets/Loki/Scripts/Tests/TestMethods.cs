using Loki.Runtime.Attributes;

namespace Loki.Tests
{
	public class TestMethods
	{
		[Loki]
		public static int TestMethod1(float inputFloat, string inputString, bool inputBoolean)
		{
			return 100;
		}
	}
}
