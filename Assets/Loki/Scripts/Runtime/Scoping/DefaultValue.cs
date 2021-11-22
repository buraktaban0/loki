namespace Loki.Runtime.Core
{
	public struct DefaultValue<T> : ILokiValue<T>
	{
		public T ValueT
		{
			get => default;
			set { }
		}
	}
}
