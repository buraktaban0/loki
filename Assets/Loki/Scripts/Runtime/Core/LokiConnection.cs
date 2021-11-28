namespace Loki.Runtime.Core
{
	[System.Serializable]
	public class LokiConnection
	{
		public string FromGuid;
		public string ToGuid;

		public LokiConnection(string fromGuid, string toGuid)
		{
			FromGuid = fromGuid;
			ToGuid = toGuid;
		}
	}
}
