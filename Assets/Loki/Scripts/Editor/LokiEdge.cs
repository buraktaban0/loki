using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using Loki.Editor;
using Loki.Editor.Utility;
using UnityEditor.Experimental.GraphView;
using UnityEngine;
using UnityEngine.UIElements;

namespace Loki.Editor
{
	public struct RenderPoint
	{
		public Vector3 position;
		public Color color;

		public RenderPoint(Vector3 position, Color color)
		{
			this.position = position;
			this.color = color;
		}
	}

	public class LokiEdge : GraphElement, ICollectibleElement
	{
		public const int RENDER_POINT_COUNT = 64;

		public const float FLAT_REGION_LENGTH = 25f;
		public const float BEZIER_CONTROL_POINT_DISTANCE = 125f;
		public const float HALF_WIDTH = 0.9f;

		public const float CONTAINS_CHECK_DISTANCE_THRESHOLD_SQR = (HALF_WIDTH * 6f) * (HALF_WIDTH * 6f);

		private static readonly Color EMPTY_COLOR = Color.white;

		private readonly RenderPoint[] renderPoints = new RenderPoint[RENDER_POINT_COUNT];

		public LokiPort fromPort { get; private set; }
		public LokiPort toPort   { get; private set; }

		public int validPortCount => fromPort == null ? 0 : toPort == null ? 1 : 2;


		private LokiGraphView graphView => this.GetFirstAncestorOfType<LokiGraphView>();

		private List<LokiPort> eligiblePorts;

		private LokiPort candidatePort;


		private float actualHalfWidth = HALF_WIDTH;

		private bool isMouseOver = false;
		private bool isSelected = false;

		public enum State
		{
			None = 0,
			Open = 1,
			Closed = 2
		}

		public State state => (State) validPortCount;

		public bool isDestroyed { get; set; } = false;


		private Vector3 mouseWorldPosition;


		public LokiEdge()
		{
			styleSheets.Add(LokiResources.Get<StyleSheet>("StyleSheets/LokiEdge.uss"));
			capabilities |= Capabilities.Selectable | Capabilities.Deletable;
			usageHints = UsageHints.DynamicTransform;
			generateVisualContent += OnGenerateVisualContent;
			RegisterCallback<MouseEnterEvent>(OnMouseEnter);
			RegisterCallback<MouseLeaveEvent>(OnMouseLeave);
			RegisterCallback<AttachToPanelEvent>(OnAttachToPanel);
			RegisterCallback<DetachFromPanelEvent>(OnDetachFromPanel);
			this.AddManipulator(new ContextualMenuManipulator(OnContextMenuPopulate));
		}

		public override bool IsSelectable()
		{
			return isMouseOver;
		}

		private void OnContextMenuPopulate(ContextualMenuPopulateEvent evt)
		{
			evt.menu.AppendAction("Delete", action => DestroySelf());
		}

		private void OnAttachToPanel(AttachToPanelEvent evt)
		{
			SendToBack();
			TriggerRepaint();
		}

		private void OnDetachFromPanel(DetachFromPanelEvent evt)
		{
			DestroySelf();
		}


		private void OnMouseLeave(MouseLeaveEvent evt)
		{
			if (state != State.Closed)
				return;
			RemoveFromClassList(LokiEditorUtility.CLASS_HOVER);
			actualHalfWidth = HALF_WIDTH;
			TriggerRepaint();
		}

		private void OnMouseEnter(MouseEnterEvent evt)
		{
			if (state != State.Closed)
				return;
			AddToClassList(LokiEditorUtility.CLASS_HOVER);
			actualHalfWidth = HALF_WIDTH * 1.5f;
			TriggerRepaint();
		}


		public override void OnSelected()
		{
			isSelected = true;

			TriggerRepaint();
			base.OnSelected();
		}

		public override void OnUnselected()
		{
			isSelected = false;

			TriggerRepaint();
			base.OnUnselected();
		}


		private void OnPortGeometryChanged(GeometryChangedEvent evt)
		{
			TriggerRepaint();
		}


		private void OnGenerateVisualContent(MeshGenerationContext cxt)
		{
			DrawEdge(cxt);
		}

		public override void HandleEvent(EventBase evt)
		{
			if (isDestroyed)
				return;

			if (state == State.Closed)
			{
				base.HandleEvent(evt);
				return;
			}

			if (evt.eventTypeId == MouseMoveEvent.TypeId())
			{
				var mouseEvent = (MouseMoveEvent) evt;
				mouseWorldPosition = mouseEvent.mousePosition;

				TriggerRepaint();
			}
			else if (evt.eventTypeId == MouseUpEvent.TypeId())
			{
				var upEvt = evt as MouseUpEvent;
				this.ReleaseMouse();

				if (candidatePort == null)
				{
					graphView.OnEdgeDroppedFree(fromPort, upEvt.mousePosition);
					DestroySelf();
				}
				else
				{
					this.Connect(fromPort, candidatePort);
				}
			}

			base.HandleEvent(evt);
		}

		public void DestroySelf()
		{
			ReleaseEligiblePorts();

			isDestroyed = true;
			this.RemoveFromHierarchy();
			fromPort?.DisconnectEdge(this);
			toPort?.DisconnectEdge(this);
			fromPort = null;
			toPort = null;
		}

		public override bool ContainsPoint(Vector2 localPoint)
		{
			for (int i = 0; i < renderPoints.Length - 1; i++)
			{
				var p0 = renderPoints[i].position;
				var p1 = renderPoints[i + 1].position;
				if (LokiGeometryUtility.PointSqrDistanceToLineSegment(localPoint, p0, p1) <=
				    CONTAINS_CHECK_DISTANCE_THRESHOLD_SQR)
				{
					isMouseOver = true;
					return true;
				}
			}

			isMouseOver = false;
			return false;
		}

		private void SetCandidatePort(MouseEnterEvent evt)
		{
			candidatePort = evt.target as LokiPort;
		}

		private void DiscardCandidatePort(MouseLeaveEvent evt)
		{
			if (candidatePort == evt.target)
				candidatePort = null;
		}

		private void RegisterPortEvents(LokiPort port)
		{
			if (port == null)
				return;
			port.GetFirstAncestorOfType<LokiNodeView>().RegisterCallback<GeometryChangedEvent>(OnPortGeometryChanged);
		}

		private void UnregisterPortEvents(LokiPort port)
		{
			if (port == null)
				return;
			port.GetFirstAncestorOfType<LokiNodeView>().UnregisterCallback<GeometryChangedEvent>(OnPortGeometryChanged);
		}

		public void Connect(LokiPort fromPort, LokiPort toPort)
		{
			if (fromPort == null)
			{
				DestroySelf();
				return;
			}

			if (fromPort == toPort)
			{
				DestroySelf();
				return;
			}

			UnregisterPortEvents(this.fromPort);
			UnregisterPortEvents(this.toPort);


			this.fromPort = fromPort;
			this.toPort = toPort;

			RegisterPortEvents(fromPort);
			RegisterPortEvents(toPort);

			switch (state) // Check if edge is closed after this Connect call, or it's still open
			{
				case State.Open:
					this.CaptureMouse();
					CollectEligiblePorts();
					break;
				case State.Closed:
					fromPort.ConnectEdge(this);
					toPort.ConnectEdge(this);
					this.ReleaseMouse();
					ReleaseEligiblePorts();
					break;
			}

			mouseWorldPosition = this.fromPort.connectionWorldPos;
			TriggerRepaint();
		}


		private void CollectEligiblePorts()
		{
			eligiblePorts = graphView.CollectEligiblePorts(fromPort);


			foreach (var port in eligiblePorts)
			{
				port.RegisterCallback<MouseEnterEvent>(SetCandidatePort);
				port.RegisterCallback<MouseLeaveEvent>(DiscardCandidatePort);
			}
		}

		private void ReleaseEligiblePorts()
		{
			if (eligiblePorts == null)
				return;

			graphView?.ReleaseEligiblePorts();

			foreach (var port in eligiblePorts)
			{
				port.UnregisterCallback<MouseEnterEvent>(SetCandidatePort);
				port.UnregisterCallback<MouseLeaveEvent>(DiscardCandidatePort);
			}

			eligiblePorts.Clear();
			eligiblePorts = null;
		}


		public void TriggerRepaint()
		{
			PrepareRenderPoints();
			MarkDirtyRepaint();
		}

		private void ReadPort(LokiPort port, out Vector3 point, out Vector3 dir, out Color color)
		{
			point = this.WorldToLocal(port.connectionWorldPos);
			dir = port.directionVec;
			color = port.color.value;
		}

		private void PrepareRenderPoints()
		{
			if (state == State.None)
				return;
			var pos = GetPosition();
			pos.xMax -= pos.xMin;
			pos.yMax -= pos.yMin;
			pos.xMin = 0f;
			pos.yMin = 0f;
			SetPosition(pos);
			Vector3 point1;
			Vector3 dir1;
			Color color1;

			ReadPort(fromPort, out var point0, out var dir0, out var color0);
			if (toPort == null)
			{
				if (candidatePort == null)
				{
					point1 = this.WorldToLocal(mouseWorldPosition);
					dir1 = (fromPort.connectionWorldPos - mouseWorldPosition);
					color1 = EMPTY_COLOR;
				}
				else
				{
					ReadPort(candidatePort, out point1, out dir1, out color1);
				}
			}

			else
			{
				ReadPort(toPort, out point1, out dir1, out color1);
			}

			if (isSelected)
			{
				color0 += Color.white * 0.5f;
				color1 += Color.white * 0.5f;
			}

			point0.z = point1.z = 0f;
			dir0.Normalize();
			dir1.Normalize();
			var distance = Vector3.Distance(point0, point1);
			var flatRegionLength = Mathf.Min(FLAT_REGION_LENGTH, distance);
			var bezierControlPointDistance = Mathf.Min(BEZIER_CONTROL_POINT_DISTANCE, distance);
			var p0 = point0 + dir0 * flatRegionLength;
			var p1 = point0 + dir0 * bezierControlPointDistance;
			var p2 = point1 + dir1 * bezierControlPointDistance;
			var p3 = point1 + dir1 * flatRegionLength;
			renderPoints[0] = new RenderPoint(point0, color0);
			renderPoints[1] = new RenderPoint(p0, color0);
			renderPoints[RENDER_POINT_COUNT - 2] = new RenderPoint(p3, color1);
			renderPoints[RENDER_POINT_COUNT - 1] = new RenderPoint(point1, color1);
			for (int i = 2; i < RENDER_POINT_COUNT - 2; i++)
			{
				var p = GetBezierRenderPoint(p0, p1, p2, p3, color0, color1, (float) i / (RENDER_POINT_COUNT - 1));
				renderPoints[i] = p;
			}

			UpdateLayout();
		}


		private void UpdateLayout()
		{
			float xMin = float.MaxValue, yMin = float.MaxValue, xMax = float.MinValue, yMax = float.MinValue;
			for (int i = 0;
				i < renderPoints.Length;
				i++)
			{
				var p = renderPoints[i];
				xMin = Mathf.Min(xMin, p.position.x);
				xMax = Mathf.Max(xMax, p.position.x);
				yMin = Mathf.Min(yMin, p.position.y);
				yMax = Mathf.Max(yMax, p.position.y);
			}

			var padding = actualHalfWidth * 8f;
			var min = new Vector3(xMin - padding, yMin - padding);

			var max = new Vector3(xMax + padding, yMax + padding);
			//min = this.parent.WorldToLocal(min);
			//max = this.parent.WorldToLocal(max);

			var size = max - min;
			var rect = new Rect(min, size);

			SetPosition(rect);
			// style.position = Position.Absolute;
			// style.top = new StyleLength(min.y);
			// style.left = new StyleLength(min.x);
			// style.width = size.x;
			// style.height = size.y;
			for (int i = 0;
				i < renderPoints.Length;
				i++)
			{
				renderPoints[i].position -= min;
			}
		}

		private void DrawEdge(MeshGenerationContext cxt)
		{
			if (state == State.None)
				return;
			uint vertexCount = RENDER_POINT_COUNT * 2;
			uint indexCount = (vertexCount - 2) * 3;
			var mesh = cxt.Allocate((int) vertexCount, (int) indexCount, null);

			Vector3 normal = default;
			for (int i = 0;
				i < RENDER_POINT_COUNT;
				i++)
			{
				if (i < RENDER_POINT_COUNT - 1)
				{
					if (i > 0)
					{
						normal = (renderPoints[i + 1].position - renderPoints[i - 1].position).normalized *
						         actualHalfWidth;
					}
					else
					{
						normal = (renderPoints[i + 1].position - renderPoints[i].position).normalized * actualHalfWidth;
					}

					var tmp = normal.x;
					normal.x = normal.y;
					normal.y = -tmp;
				}

				var p = renderPoints[i];

				var v0 = GetVertex(p.position + normal, p.color);
				var v1 = GetVertex(p.position - normal, p.color);

				mesh.SetNextVertex(v0);
				mesh.SetNextVertex(v1);
			}

			for (int i = 0; i < RENDER_POINT_COUNT - 1; i++)
			{
				mesh.SetNextIndex((ushort) (0 + i * 2));
				mesh.SetNextIndex((ushort) (2 + i * 2));
				mesh.SetNextIndex((ushort) (1 + i * 2));
				mesh.SetNextIndex((ushort) (1 + i * 2));
				mesh.SetNextIndex((ushort) (2 + i * 2));
				mesh.SetNextIndex((ushort) (3 + i * 2));
			}
		}


		private Vertex GetVertex(Vector3 position, Color color)
		{
			position.z = Vertex.nearZ;
			return new Vertex
			{
				position = position,
				tint = color
			};
		}

		private RenderPoint GetBezierRenderPoint(Vector3 p0, Vector3 p1, Vector3 p2, Vector3 p3, Color color0,
		                                         Color color1,
		                                         float t)
		{
			t = Mathf.SmoothStep(0f, 1f, Mathf.Clamp01((t)));
			var p01 = Vector3.Lerp(p0, p1, t);
			var p12 = Vector3.Lerp(p1, p2, t);
			var p23 = Vector3.Lerp(p2, p3, t);
			var p02 = Vector3.Lerp(p01, p12, t);
			var p13 = Vector3.Lerp(p12, p23, t);
			var p = Vector3.Lerp(p02, p13, t);

			var c = Color.Lerp(color0, color1, t);
			return new RenderPoint
			{
				position = p,
				color = c
			};
		}

		public void CollectElements(HashSet<GraphElement> collectedElementSet, Func<GraphElement, bool> conditionFunc)
		{
			collectedElementSet.Add(this);
		}
	}
}
