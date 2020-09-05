using System;
using Loki.Runtime;
using UnityEditor;
using UnityEditor.UIElements;
using UnityEngine;
using UnityEngine.UIElements;

namespace UniNode.Scripts.Editor
{
	public class LokiGraphWindow : EditorWindow
	{
		private LokiGraphView _graphView;

		[MenuItem("Loki/Debug Window")]
		public static LokiGraphWindow GetWindow()
		{
			var window = EditorWindow.GetWindow<LokiGraphWindow>();
			window.titleContent = new GUIContent("Loki");
			return window;
		}


		public static void EditGraph(LokiGraph graph)
		{
			Debug.Log("Editing " + graph.GetName());
		}


		private void OnEnable()
		{
			Debug.Log("LokiGraphWindow opened");

			SetupGraphView();
			SetupToolbar();
		}

		private void OnDestroy()
		{
			Debug.Log("LokiGraphWindow closed");
		}


		private void SetupToolbar()
		{
			var toolbar = new Toolbar();

			rootVisualElement.Add(toolbar);
		}

		private void SetupGraphView()
		{
			_graphView = new LokiGraphView();
			
			_graphView.StretchToParentSize();

			rootVisualElement.Add(_graphView);
		}
	}
}
