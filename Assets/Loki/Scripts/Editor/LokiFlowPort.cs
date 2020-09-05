using System;
using UnityEditor.Experimental.GraphView;
using UnityEngine;
using UnityEngine.UIElements;

namespace Loki.Editor
{
	public class LokiFlowPort : LokiPort
	{
		public override StyleColor color
		{
			get { return cap.resolvedStyle.unityBackgroundImageTintColor; }
			set
			{
				cap.style.unityBackgroundImageTintColor = value;
				capBorder.style.unityBackgroundImageTintColor = value;
			}
		}

		public LokiFlowPort(Orientation portOrientation, Direction portDirection, Capacity capacity,
		                    string name = "Unnamed Port") : base(portOrientation, portDirection, capacity, name)
		{
			var ss = LokiResources.Get<StyleSheet>("StyleSheets/LokiFlowPort.uss");

			styleSheets.Add(ss);
			
			defaultColor = new Color(0f, 0.62f, 0.79f, 0.82f);
			color = defaultColor;

			generateVisualContent += OnGenerateVisualContent;
		}


		private void OnGenerateVisualContent(MeshGenerationContext obj)
		{
		}
	}
}
