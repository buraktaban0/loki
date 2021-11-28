namespace Loki.Runtime.Core
{
	public struct LokiValue<T> : ILokiValue<T>
	{
		public T Value { get; set; }

		public LokiValue(T value)
		{
			Value = value;
		}

	}
}
