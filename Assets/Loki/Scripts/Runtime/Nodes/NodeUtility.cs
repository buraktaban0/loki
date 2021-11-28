using Loki.Runtime.Utility;

namespace Loki.Runtime.Nodes
{
	public static class NodeUtility
	{
		private const int NODE_GUID_LENGTH = 6;

		public static T CreateNode<T>() where T : ILokiNode, new()
		{
			var node = new T
			           {
				           Guid = RandomString.Get(NODE_GUID_LENGTH)
			           };
			return node;
		}
	}
}
