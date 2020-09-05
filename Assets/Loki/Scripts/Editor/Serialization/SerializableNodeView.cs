using UnityEngine;

namespace Loki.Editor.Serialization
{
	public class SerializableNodeView
	{
		public string guid;
		
		public Vector2 position;

		public SerializableNodeView(LokiNodeView view)
		{
			this.guid = view.guid;
			this.position = view.GetPosition().position;
		}
	}
}
