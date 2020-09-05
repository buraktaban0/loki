using System;
using System.Collections.Generic;
using System.Linq;
using Loki.Editor.Utility;
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
		private readonly VisualElement capBorder;
		private readonly VisualElement cap;

		private readonly VisualElement connectionPoint;

		public override bool canGrabFocus => true;

		public List<LokiEdge> edges = new List<LokiEdge>();

		public LokiGraphView graphView => this.GetFirstAncestorOfType<LokiGraphView>();

		public Color color
		{
			get => cap.resolvedStyle.backgroundColor;
			set
			{
				cap.style.backgroundColor = value;
				capBorder.style.borderTopColor = value;
				capBorder.style.borderRightColor = value;
				capBorder.style.borderBottomColor = value;
				capBorder.style.borderLeftColor = value;
			}
		}

		public Vector3 directionVec { get; private set; }

		public Vector3 connectionWorldPos
		{
			get { return cap.parent.LocalToWorld(cap.layout.center); }
		}

		public int  connectionCount => edges.Count;
		public bool hasConnections  => connectionCount > 0;

		public bool hasCapacity => connectionCount < (int) capacity;

		public Capacity capacity;

		public LokiPort(Orientation portOrientation, Direction portDirection, Capacity capacity,
		                string name = "Unnamed Port")
		{
			var vst = LokiResources.Get<VisualTreeAsset>("UXML/LokiPort.uxml");
			var ss = LokiResources.Get<StyleSheet>("StyleSheets/LokiPort.uss");

			vst.CloneTree(this);

			this.styleSheets.Add(ss);

			this.capacity = capacity;
			this.name = name;

			this.pickingMode = PickingMode.Position;

			directionVec = portDirection == Direction.Input ? Vector3.left : Vector3.right;

			focusable = true;

			connectionPoint = this.Q<VisualElement>("connection-point");

			capBorder = this.Q<VisualElement>("outline-border");
			cap = this.Q<VisualElement>("cap");

			cap.visible = false;

			connectionPoint.RegisterCallback<MouseEnterEvent>(OnMouseEnterConnection);
			connectionPoint.RegisterCallback<MouseLeaveEvent>(OnMouseLeaveConnection);

			connectionPoint.RegisterCallback<MouseDownEvent>(OnMouseDownConnection);
		}

		protected virtual void OnMouseEnterConnection(MouseEnterEvent evt)
		{
			cap.AddToClassList(LokiEditorUtility.CLASS_HOVER);
			capBorder.AddToClassList(LokiEditorUtility.CLASS_HOVER);

			RefreshPortState();
		}

		protected virtual void OnMouseLeaveConnection(MouseLeaveEvent evt)
		{
			cap.RemoveFromClassList(LokiEditorUtility.CLASS_HOVER);
			capBorder.RemoveFromClassList(LokiEditorUtility.CLASS_HOVER);

			RefreshPortState();
		}

		protected virtual void OnMouseDownConnection(MouseDownEvent evt)
		{
			evt.StopImmediatePropagation();
			StartConnection();
		}

		public void SetColor(Color color)
		{
			capBorder.style.color = color;
			cap.style.backgroundColor = color;


			RefreshPortState();
		}

		public void RefreshPortState()
		{
			cap.visible = hasConnections;

			this.RunLater(() =>
			{
				foreach (var edge in edges)
				{
					edge.TriggerRepaint();
				}
			});
		}

		public void StartConnection()
		{
			if (!hasCapacity)
				return;

			var edge = new LokiEdge();
			graphView.AddElement(edge);
			edge.Connect(this, null);

			RefreshPortState();
		}

		public bool ConnectEdge(LokiEdge edge)
		{
			if (!hasCapacity)
				return false;
			
			if (!edges.Contains(edge))
				edges.Add(edge);

			RefreshPortState();

			return true;
		}

		public void DisconnectEdge(LokiEdge edge)
		{
			if (edges.Contains(edge))
				edges.Remove(edge);

			RefreshPortState();
		}
	}
}
