namespace Loki.Runtime.Core
{
	[System.Serializable]
	public class LokiConnection
	{
		public string FromGuid;
		public string FromField;
		public string ToGuid;
		public string ToField;

		public LokiConnection(string fromGuid, string fromField, string toGuid, string toField)
		{
			FromGuid = fromGuid;
			FromField = fromField;
			ToGuid = toGuid;
			ToField = toField;
		}
	}
}
