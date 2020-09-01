using System.Collections;
using System.Collections.Generic;
using Loki.Editor;
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
		this.AddElement(node);
		
		var nodeView = new LokiNodeView();
		nodeView.SetPosition(new Rect(new Vector2(300, 300), Vector2.zero));
		
		this.AddElement(nodeView);
	}
}
