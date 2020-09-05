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
		private static readonly Color INACTIVE_COLOR = new Color(1f, 1f, 1f, 0.25f);

		public readonly VisualElement capBorder;
		public readonly VisualElement cap;

		public readonly VisualElement connectionElement;

		public LokiNodeView node;

		public override bool canGrabFocus => true;

		public List<LokiEdge> edges = new List<LokiEdge>();

		public LokiGraphView graphView => this.GetFirstAncestorOfType<LokiGraphView>();

		public Color defaultColor;

		public virtual StyleColor color
		{
			get => cap.resolvedStyle.backgroundColor;
			set
			{
				cap.style.backgroundColor = value;
				capBorder.style.borderTopColor = value;
				capBorder.style.borderRightColor = value;
				capBorder.style.borderBottomColor = value;
				capBorder.style.borderLeftColor = value;

				RefreshPortState();
			}
		}

		private bool _active = true;

		public virtual bool active
		{
			get => _active;
			set
			{
				_active = value;

				this.SetEnabled(value);

				if (_active)
				{
					this.color = defaultColor;
				}
				else
				{
					this.color = INACTIVE_COLOR;
				}
				
				RefreshPortState();
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
			connectionElement = this.Q<VisualElement>("connection-point");

			capBorder = this.Q<VisualElement>("outline-border");
			cap = this.Q<VisualElement>("cap");

			cap.visible = false;

			cap.pickingMode = PickingMode.Ignore;
			capBorder.pickingMode = PickingMode.Ignore;

			this.color = Color.red;

			this.RegisterCallback<AttachToPanelEvent>(OnAttachToPanel);

			connectionElement.RegisterCallback<MouseEnterEvent>(OnMouseEnterConnection);
			connectionElement.RegisterCallback<MouseLeaveEvent>(OnMouseLeaveConnection);

			connectionElement.RegisterCallback<MouseDownEvent>(OnMouseDownConnection);

			connectionElement.focusable = true;
			connectionElement.pickingMode = PickingMode.Position;
			connectionElement.AddManipulator(new ContextualMenuManipulator(OnContextMenuPopulate));
		}

		private void OnAttachToPanel(AttachToPanelEvent evt)
		{
			node = this.GetFirstAncestorOfType<LokiNodeView>();
		}

		protected virtual void OnMouseEnterConnection(MouseEnterEvent evt)
		{
			cap.AddToClassList(LokiEditorUtility.CLASS_HOVER);
			capBorder.AddToClassList(LokiEditorUtility.CLASS_HOVER);

			this.color = new StyleColor(StyleKeyword.Initial);

			RefreshPortState();
		}

		protected virtual void OnMouseLeaveConnection(MouseLeaveEvent evt)
		{
			cap.RemoveFromClassList(LokiEditorUtility.CLASS_HOVER);
			capBorder.RemoveFromClassList(LokiEditorUtility.CLASS_HOVER);

			this.color = defaultColor;


			RefreshPortState();
		}

		protected virtual void OnMouseDownConnection(MouseDownEvent evt)
		{
			if (evt.button == 0)
			{
				evt.StopImmediatePropagation();
				StartConnection();
			}
		}

		private void OnContextMenuPopulate(ContextualMenuPopulateEvent evt)
		{
			Debug.Log("context menu");
			evt.menu.AppendAction("Disconnect All", action => DisconnectAllEdges(), DropdownMenuAction.Status.Normal);
		}

		public void SetColor(Color color)
		{
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
			var edge = new LokiEdge();
			graphView.AddElement(edge);
			edge.Connect(this, null);

			RefreshPortState();
		}

		public bool ConnectEdge(LokiEdge edge)
		{
			if (!hasCapacity)
			{
				edges.First().DestroySelf();
			}

			if (!edges.Contains(edge))
				edges.Add(edge);

			RefreshPortState();

			return true;
		}

		public void DisconnectEdge(LokiEdge edge)
		{
			if (edges.Contains(edge))
				edges.Remove(edge);

			if (edge.isDestroyed == false)
				edge.DestroySelf();

			RefreshPortState();
		}


		private void DisconnectAllEdges()
		{
			for (int i = 0; i < edges.Count; i++)
			{
				DisconnectEdge(edges[i]);
			}

			RefreshPortState();
		}
	}
}
