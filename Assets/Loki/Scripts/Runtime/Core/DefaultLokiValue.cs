namespace Loki.Runtime.Core
{
	public struct DefaultLokiValue<T> : ILokiValue<T>
	{
		public T ValueT
		{
			get => default;
			set { }
		}
	}
}
