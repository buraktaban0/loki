using System;
using UnityEditor.Experimental.GraphView;
using UnityEngine;
using UnityEngine.UIElements;

namespace Loki.Editor
{
	public enum Capacity : int
	{
		None = 0,
		Single = 1,
		Multi = 64
	}

	public class LokiPort : VisualElement
	{
		public LokiPort(Orientation portOrientation, Direction portDirection, Capacity portCapacity)
		{
			var vst = LokiResources.Get<VisualTreeAsset>("UXML/LokiPort.uxml");
			var ss = LokiResources.Get<StyleSheet>("StyleSheets/LokiPort.uss");

			vst.CloneTree(this);

			this.styleSheets.Add(ss);

			this.name = "Dummy Port";

			var portRoot = this.Q<VisualElement>("port-root");
			portRoot.StretchToParentSize();

			this.pickingMode = PickingMode.Position;


			//RegisterCallback<MouseEnterEvent>(ev => { Debug.Log("Mouse enter"); });
		}
	}
}
