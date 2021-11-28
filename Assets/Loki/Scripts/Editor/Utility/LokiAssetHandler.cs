using Loki.Runtime;
using Loki.Runtime.Core;
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
			LokiGraphAsset graph = EditorUtility.InstanceIDToObject(instanceID) as LokiGraphAsset;

			if (graph == null)
				return false;

			LokiGraphWindow.EditGraph(graph.Graph);

			return true;
		}
	}
}
