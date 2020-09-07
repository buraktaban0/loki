using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.Linq;
using System.Reflection;
using Loki.Editor;
using Loki.Editor.Adapters;
using Loki.Runtime.Utility;
using Unity.Collections;
using Unity.Mathematics;
using UnityEditor;
using UnityEditor.Experimental.GraphView;
using UnityEngine;
using UnityEngine.UIElements;
using Debug = UnityEngine.Debug;
using Object = UnityEngine.Object;

public class TestClass
{
	public float x;
	public string y;
}

public class LokiGraphView : GraphView
{
	private static readonly string STYLE_SHEET_PATH = "StyleSheets/LokiStyle.uss";


	protected override bool canPaste              => true;
	protected override bool canCopySelection      => true;
	protected override bool canCutSelection       => canCopySelection;
	protected override bool canDeleteSelection    => canCopySelection;
	protected override bool canDuplicateSelection => canCopySelection;


	public new List<LokiPort> ports => this.Query<LokiPort>().ToList();
	public new List<LokiEdge> edges => this.Query<LokiEdge>().ToList();

	public SearchWindowProvider searchWindowProvider;


	public LokiGraphView()
	{
		this.AddManipulator(new ClickSelector());
		this.AddManipulator(new ContentDragger());
		this.AddManipulator(new SelectionDragger());
		this.AddManipulator(new RectangleSelector());

		SetupZoom(ContentZoomer.DefaultMinScale, ContentZoomer.DefaultMaxScale * 2f);

		var styleSheet = LokiResources.Get<StyleSheet>(STYLE_SHEET_PATH);
		styleSheets.Add(styleSheet);

		searchWindowProvider = ScriptableObject.CreateInstance<SearchWindowProvider>();
		searchWindowProvider.Prepare();

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


	public List<LokiPort> CollectEligiblePorts(LokiPort fromPort)
	{
		var otherPorts = ports.Where(p => p != fromPort).ToList();

		var eligiblePorts = otherPorts.Where(port => port.node != fromPort.node)
		                              .ToList();


		foreach (var port in ports.Except(eligiblePorts).ToList())
		{
			port.active = false;
		}

		return eligiblePorts;
	}

	public void ReleaseEligiblePorts()
	{
		foreach (var port in ports)
		{
			port.active = true;
		}
	}

	public void OnEdgeDroppedFree(LokiPort fromPort, Vector2 mousePos)
	{
		mousePos = GUIUtility.GUIToScreenPoint(mousePos);

		LokiSearchWindow.Open(new LokiSearchTreeProvider(), mousePos);

		return;
		Debug.Log("a");
		searchWindowProvider.fromPort = fromPort;
		SearchWindow.Open(new SearchWindowContext(mousePos), searchWindowProvider);
		Debug.Log("b");
	}

	public void OnDestroy()
	{
		Object.DestroyImmediate(searchWindowProvider);
	}
}
