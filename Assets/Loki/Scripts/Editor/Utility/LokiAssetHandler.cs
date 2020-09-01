using UniNode.Scripts.Editor;
using UnityEditor;
using UnityEditor.Callbacks;

namespace Loki.Editor.Utility
{
	public static class LokiAssetHandler
	{
		[OnOpenAsset(1)]
		public static bool OnAssetOpened(int instanceID, int line)
		{
			LokiGraph graph = EditorUtility.InstanceIDToObject(instanceID) as LokiGraph;

			if (graph == null)
				return false;

			LokiGraphWindow.EditAsset(graph);

			return true;
		}
	}
}
