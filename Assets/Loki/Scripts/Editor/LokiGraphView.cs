using System.Collections;
using System.Collections.Generic;
using Loki.Editor;
using Loki.Scripts.Editor;
using UnityEditor;
using UnityEditor.Experimental.GraphView;
using UnityEngine;
using UnityEngine.UIElements;

public class LokiGraphView : GraphView
{
	private static readonly string STYLE_SHEET_PATH = "Assets/Loki/Res/StyleSheets/LokiStyle.uss";

	public LokiGraphView()
	{
		this.AddManipulator(new ClickSelector());
		this.AddManipulator(new ContentDragger());
		this.AddManipulator(new SelectionDragger());
		this.AddManipulator(new RectangleSelector());

		SetupZoom(ContentZoomer.DefaultMinScale, ContentZoomer.DefaultMaxScale);

		var styleSheet = EditorGUIUtility.Load(STYLE_SHEET_PATH) as StyleSheet;
		styleSheets.Add(styleSheet);

		var node = new Node();

		var port = node.InstantiatePort(Orientation.Horizontal, Direction.Input, Port.Capacity.Multi, typeof(float));
		node.inputContainer.Add(port);

		this.AddElement(node);

		var nodeView = new LokiNodeView();
		nodeView.SetPosition(Vector2.one * 400f);

		this.AddElement(nodeView);


		var edge = new LokiEdge();
		this.AddElement(edge);
	}
}
