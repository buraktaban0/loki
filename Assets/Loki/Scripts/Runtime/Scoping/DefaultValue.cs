namespace Loki.Runtime.Core
{
	public struct DefaultValue<T> : ILokiValue<T>
	{
		public T Value
		{
			get => default;
			set { }
		}
	}
}
