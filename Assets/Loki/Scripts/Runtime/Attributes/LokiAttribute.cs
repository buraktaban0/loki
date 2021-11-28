using System;

namespace Loki.Runtime.Attributes
{
	[AttributeUsage(AttributeTargets.Method)]
	public class LokiAttribute : Attribute
	{
		public string Name = null;

		public LokiAttribute()
		{
		}
	}
}
