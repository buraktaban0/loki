namespace Loki.Runtime.Core
{
	public interface ILokiScope
	{
		ILokiValue<T>      GetValue<T>(string id);
		ILokiValue<object> GetValue(string id);
		void               SetValue<T>(string id, ILokiValue<T> value);
		bool               HasValue(string name);
	}
}
