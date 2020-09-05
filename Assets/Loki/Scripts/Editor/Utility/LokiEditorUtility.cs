using System;
using UnityEditor.Experimental.GraphView;
using UnityEngine;
using UnityEngine.UIElements;

namespace Loki.Editor.Utility
{
	public static class LokiEditorUtility
	{
		public const string CLASS_HOVER = "hover";
		public const string CLASS_SELECTED = "selected";

		public static void SetPosition(this GraphElement element, Vector2 pos)
		{
			var rect = element.GetPosition();
			var size = rect.size;
			rect.xMin = pos.x;
			rect.yMin = pos.y;
			rect.size = size;
			element.SetPosition(rect);
		}


		public static LokiPort InsertPort(this VisualElement container, int index, Orientation orientation,
		                                  Direction direction,
		                                  Capacity capacity,
		                                  string name)
		{
			var port = new LokiFlowPort(orientation, direction, capacity, name);
			container.Insert(index, port);
			return port;
		}

		public static LokiPort AddPort(this VisualElement container, Orientation orientation, Direction direction,
		                               Capacity capacity,
		                               string name) => InsertPort(container, container.childCount, orientation, direction, capacity, name);

		public static void RunLater(this VisualElement element, Action action, long delayMs = 1)
		{
			var exec = element.schedule.Execute(action);
			exec.ExecuteLater(delayMs);
		}
		
		
		
	}
}
