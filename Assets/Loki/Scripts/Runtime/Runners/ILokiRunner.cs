namespace Loki.Runtime.Core
{
	public interface ILokiRunner
	{
		public void Run();

		object GetData(string name);

		T GetData<T>(string name);

		void SetData<T>(string name, T value);

		bool HasData(string name);

		void SetGraph(ILokiGraph graph);
	}
}
