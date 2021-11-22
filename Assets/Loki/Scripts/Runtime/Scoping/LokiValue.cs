namespace Loki.Runtime.Core
{
	public struct LokiValue<T> : ILokiValue<T>
	{
		public T ValueT { get; set; }
		
	}
}
