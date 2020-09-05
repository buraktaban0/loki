using Loki.Runtime;
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
			LokiGraphAsset graphAsset = EditorUtility.InstanceIDToObject(instanceID) as LokiGraphAsset;

			if (graphAsset == null)
				return false;

			LokiGraphWindow.EditGraph(graphAsset.graph);

			return true;
		}
	}
}
