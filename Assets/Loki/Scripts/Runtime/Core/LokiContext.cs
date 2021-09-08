using System;

namespace Loki.Runtime
{
	public struct LokiContext
	{
		public enum Type : int
		{
			None = 0,
			Object = 1,
			Scene = 2,
			Global = 3,
			All = 4
		}

		public Type type { get; set; }

		public LokiContext(Type type)
		{
			this.type = type;
		}

		public bool FitsInto(LokiContext other)
		{
			return other.type >= this.type;
		}

		public static implicit operator LokiContext(Type type)
		{
			return new LokiContext {type = type};
		}

		public static implicit operator Type(LokiContext cxt)
		{
			return cxt.type;
		}
	}
}
