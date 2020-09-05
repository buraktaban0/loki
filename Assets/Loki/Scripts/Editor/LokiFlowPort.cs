using System;
using UnityEditor.Experimental.GraphView;
using UnityEngine.UIElements;

namespace Loki.Editor
{
	public class LokiFlowPort : LokiPort
	{
		public LokiFlowPort(Orientation portOrientation, Direction portDirection, Capacity capacity,
		                    string name = "Unnamed Port") : base(portOrientation, portDirection, capacity, name)
		{
			var ss = LokiResources.Get<StyleSheet>("StyleSheets/LokiFlowPort.uss");

			styleSheets.Add(ss);

			generateVisualContent += OnGenerateVisualContent;
		}


		private void OnGenerateVisualContent(MeshGenerationContext obj)
		{
		}
	}
}
