using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using Loki.Editor;
using Loki.Editor.Utility;
using UnityEditor.Experimental.GraphView;
using UnityEngine;
using UnityEngine.UIElements;

namespace Loki.Scripts.Editor
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

	public class LokiEdge : GraphElement
	{
		private const int RENDER_POINT_COUNT = 48;

		private const float FLAT_REGION_LENGTH = 20f;
		private const float BEZIER_CONTROL_POINT_DISTANCE = 120f;
		private const float HALF_WIDTH = 1.5f;

		private const float CONTAINS_CHECK_DISTANCE_THRESHOLD_SQR = (HALF_WIDTH * 5f) * (HALF_WIDTH * 5f);

		private const string HOVER_CLASS_NAME = "hover";

		private static readonly Color EMPTY_COLOR = Color.gray;

		private readonly RenderPoint[] renderPoints = new RenderPoint[RENDER_POINT_COUNT];


		private float actualHalfWidth = HALF_WIDTH;

		public LokiPort port0 { get; private set; }
		public LokiPort port1 { get; private set; }

		private Vector3 point0
		{
			get
			{
				if (port0 == null)
					return Vector3.one * 150f;

				return this.WorldToLocal(port0.connectionWorldPos);
			}
		}

		private Vector3 point1
		{
			get
			{
				if (port1 == null)
					return Vector3.one * 250f;

				return this.WorldToLocal(port1.connectionWorldPos);
			}
		}

		private Vector3 direction0
		{
			get
			{
				if (port0 == null)
					return Vector3.up;

				return port0.directionVec;
			}
		}

		private Vector3 direction1
		{
			get
			{
				if (port1 == null)
					return Vector3.down;

				return port1.directionVec;
			}
		}

		private Color color0
		{
			get
			{
				if (port0 == null)
					return EMPTY_COLOR;

				return port0.color;
			}
		}

		private Color color1
		{
			get
			{
				if (port1 == null)
					return EMPTY_COLOR;

				return port1.color;
			}
		}

		public LokiEdge()
		{
			styleSheets.Add(LokiResources.Get<StyleSheet>("StyleSheets/LokiEdge.uss"));

			generateVisualContent += GenerateVisualContent;

			RegisterCallback<MouseEnterEvent>(OnMouseEnter);
			RegisterCallback<MouseLeaveEvent>(OnMouseLeave);

			SendToBack();
		}

		private void OnMouseLeave(MouseLeaveEvent evt)
		{
			//RemoveFromClassList(HOVER_CLASS_NAME);
			actualHalfWidth = HALF_WIDTH;
			MarkDirtyRepaint();
		}

		private void OnMouseEnter(MouseEnterEvent evt)
		{
			//AddToClassList(HOVER_CLASS_NAME);
			actualHalfWidth = HALF_WIDTH * 2f;
			MarkDirtyRepaint();
		}


		private void OnPortGeometryChanged(GeometryChangedEvent evt)
		{
			SendToBack();

			PrepareVertices(point0, direction0, Color.cyan, point1, direction1, Color.red);

			MarkDirtyRepaint();
		}

		public override bool ContainsPoint(Vector2 localPoint)
		{
			for (int i = 0; i < renderPoints.Length - 1; i++)
			{
				var p0 = renderPoints[i].position;
				var p1 = renderPoints[i + 1].position;
				if (GeoUtil.PointSqrDistanceToLineSegment(localPoint, p0, p1) <= CONTAINS_CHECK_DISTANCE_THRESHOLD_SQR)
				{
					return true;
				}
			}

			return false;
		}

		public void Connect(LokiPort port0, LokiPort port1)
		{
			this.port0?.GetFirstAncestorOfType<LokiNodeView>()
			    .UnregisterCallback<GeometryChangedEvent>(OnPortGeometryChanged);
			this.port1?.GetFirstAncestorOfType<LokiNodeView>()
			    .UnregisterCallback<GeometryChangedEvent>(OnPortGeometryChanged);

			this.port0 = port0;
			this.port1 = port1;

			this.port0?.GetFirstAncestorOfType<LokiNodeView>()
			    .RegisterCallback<GeometryChangedEvent>(OnPortGeometryChanged);
			this.port1?.GetFirstAncestorOfType<LokiNodeView>()
			    .RegisterCallback<GeometryChangedEvent>(OnPortGeometryChanged);

			MarkDirtyRepaint();
		}


		private void UpdateLayout()
		{
			float xMin = float.MaxValue, yMin = float.MaxValue, xMax = float.MinValue, yMax = float.MinValue;
			for (int i = 0; i < renderPoints.Length; i++)
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

			style.position = Position.Absolute;
			style.top = new StyleLength(style.top.value.value + min.y);
			style.left = new StyleLength(style.left.value.value + min.x);
			style.width = size.x;
			style.height = size.y;


			for (int i = 0; i < renderPoints.Length; i++)
			{
				renderPoints[i].position -= min;
			}
		}


		private void PrepareVertices(Vector3 point0, Vector3 dir0, Color color0, Vector3 point1, Vector3 dir1,
		                             Color color1)
		{
			point0.z = point1.z = 0f;
			dir0.Normalize();
			dir1.Normalize();

			var p0 = point0 + dir0 * FLAT_REGION_LENGTH;
			var p1 = point0 + dir0 * BEZIER_CONTROL_POINT_DISTANCE;
			var p2 = point1 + dir1 * BEZIER_CONTROL_POINT_DISTANCE;
			var p3 = point1 + dir1 * FLAT_REGION_LENGTH;


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

		private void GenerateVisualContent(MeshGenerationContext cxt)
		{
			DrawEdge(cxt);
		}


		private void DrawEdge(MeshGenerationContext cxt)
		{
			uint vertexCount = RENDER_POINT_COUNT * 2;
			uint indexCount = (vertexCount - 2) * 3;

			var mesh = cxt.Allocate((int) vertexCount, (int) indexCount, null);

			Vector3 normal = default;
			for (int i = 0; i < RENDER_POINT_COUNT; i++)
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
	}
}
