using System;

namespace Loki.Runtime.Attributes
{
	[AttributeUsage(AttributeTargets.Class)]
	public class UseRunnerAttribute : Attribute
	{
		public Type RunnerType;

		public UseRunnerAttribute(Type runnerType)
		{
			RunnerType = runnerType;
		}
	}
}
