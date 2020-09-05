using System;
using System.Collections;
using System.Collections.Generic;
using Loki.Editor;
using Loki.Editor.Adapters;
using UnityEditor;
using UnityEditor.Experimental.GraphView;
using UnityEngine;
using UnityEngine.UIElements;

public class TestClass
{
	public float x;
	public string y;
}

public class LokiGraphView : GraphView
{
	private static readonly string STYLE_SHEET_PATH = "Assets/Loki/Res/StyleSheets/LokiStyle.uss";


	protected override bool canPaste              => true;
	protected override bool canCopySelection      => true;
	protected override bool canCutSelection       => canCopySelection;
	protected override bool canDeleteSelection    => canCopySelection;
	protected override bool canDuplicateSelection => canCopySelection;


	public new List<LokiPort> ports => this.Query<LokiPort>().ToList();
	public new List<LokiEdge> edges => this.Query<LokiEdge>().ToList();

	public LokiGraphView()
	{
		this.AddManipulator(new ClickSelector());
		this.AddManipulator(new ContentDragger());
		this.AddManipulator(new SelectionDragger());
		this.AddManipulator(new RectangleSelector());

		SetupZoom(ContentZoomer.DefaultMinScale, ContentZoomer.DefaultMaxScale * 2f);

		var styleSheet = EditorGUIUtility.Load(STYLE_SHEET_PATH) as StyleSheet;
		styleSheets.Add(styleSheet);

		var node = new Node();

		var port = node.InstantiatePort(Orientation.Horizontal, Direction.Input, Port.Capacity.Multi, typeof(float));
		node.inputContainer.Add(port);

		this.AddElement(node);


		var type = typeof(TestClass);
		var f0 = type.GetField("x");
		var f1 = type.GetField("y");

		var nodeView2 = new LokiNodeView(new VariableAdapter(f0));
		nodeView2.SetPosition(Vector2.one * 300f);

		var nodeView = new LokiNodeView(new VariableAdapter(f1));
		nodeView.SetPosition(Vector3.right * 200f + Vector3.up * 100f);

		this.AddElement(nodeView);
		this.AddElement(nodeView2);


		// EventCallback<GeometryChangedEvent> func = null;
		// func = evt =>
		// {
		// 	var edge = new LokiEdge();
		// 	edge.Connect(nodeView.Q<LokiPort>(), null); // nodeView2.Q<LokiPort>());
		// 	this.AddElement(edge);
		// 	nodeView.UnregisterCallback<GeometryChangedEvent>(func);
		// };
		//
		// nodeView.RegisterCallback<GeometryChangedEvent>(func);
	}


	public List<LokiPort> GetEligiblePorts(LokiPort fromPort)
	{
		var ports = new List<LokiPort>();

		ports.AddRange(this.ports);

		return ports;
	}
}
