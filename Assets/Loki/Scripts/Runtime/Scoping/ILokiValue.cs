namespace Loki.Runtime.Core
{
	public interface ILokiValue<T>
	{
		public T Value { get; set; }
	}
}
