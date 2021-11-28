using System;

namespace Loki.Runtime.Exceptions
{
	public class RunnerNotFoundException : Exception
	{
		public Type GraphType;

		public RunnerNotFoundException(Type graphType)
		{
			GraphType = graphType;
		}

		public override string Message => $"Runner for graph type {GraphType.FullName} could not be found.";
	}
}
