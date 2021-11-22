namespace Loki.Runtime.Core
{
	public interface ILokiValue
	{
		public object Value { get; set; }
	}

	public interface ILokiValue<T>
	{
		public T ValueT { get; set; }
	}
}
