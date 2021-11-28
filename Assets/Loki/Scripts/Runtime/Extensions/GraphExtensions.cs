using System;
using Loki.Runtime.Core;
using Loki.Runtime.Database;
using Loki.Runtime.Exceptions;

namespace Loki.Runtime.Extensions
{
	public static class GraphExtensions
	{
		public static ILokiRunner GetRunner(this ILokiGraph graph)
		{
			var graphType = graph.GetType();
			var db = LokiDatabase.Get();
			var runnerType = db.GetRunnerTypeForGraphType(graphType);
			if (runnerType == null)
			{
				throw new RunnerNotFoundException(graphType);
			}

			var runner = Activator.CreateInstance(runnerType) as ILokiRunner;

			if (runner is null)
			{
				throw new InvalidCastException(
					$"Runner for graph type {graphType.FullName} was found ({runnerType.FullName}) but it does not implement ILokiRunner interface.");
			}

			runner.SetGraph(graph);
			return runner;
		}
	}
}
