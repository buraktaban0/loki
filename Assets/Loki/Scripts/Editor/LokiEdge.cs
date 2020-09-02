using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using UnityEditor.Experimental.GraphView;
using UnityEngine;
using UnityEngine.UIElements;

namespace Loki.Scripts.Editor
{
	public class LokiEdge : GraphElement
	{
		private const int RENDER_POINT_COUNT = 16;

		private readonly Vector3[] renderPoints = new Vector3[RENDER_POINT_COUNT];

		private MethodInfo internalAllocMethod;

		public LokiEdge()
		{
			generateVisualContent += GenerateVisualContent;

			internalAllocMethod =
				typeof(MeshGenerationContext).GetMethods().Where(m => m.Name == "Allocate").ToArray()[1];
		}

		private void GenerateVisualContent(MeshGenerationContext cxt)
		{
			Vector3 p0 = Vector3.one * 300;
			var p1 = Vector3.one * 400 + Vector3.right * 150;

			PrepareRenderPoints(p0, p1);

			DrawEdge(cxt);
		}


		private void PrepareRenderPoints(Vector3 p0, Vector3 p1)
		{
			p0.z = p1.z = 0f;

			Vector3 step = (p1 - p0) / (RENDER_POINT_COUNT - 1);

			for (int i = 0; i < RENDER_POINT_COUNT; i++)
			{
				var p = p0 + step * i;
				p = parent.ChangeCoordinatesTo(this, p);
				renderPoints[i] = p;
			}
		}

		private void DrawEdge(MeshGenerationContext cxt)
		{
			uint vertexCount = RENDER_POINT_COUNT * 2;
			uint indexCount = (vertexCount - 2) * 3;
			var mesh = (MeshWriteData) internalAllocMethod.Invoke(cxt, new object[]
			{
				vertexCount,
				indexCount,
				null,
				null,
				1
			});

			Vector3[] vertexPoints = new Vector3[vertexCount];
			for (int i = 0; i < RENDER_POINT_COUNT; i++)
			{
				var p = renderPoints[i];
				vertexPoints[i * 2 + 0] = new Vector3(p.x, p.y, Vertex.nearZ);
			}

			for (int i = 0; i < renderPoints.Length; i++)
			{
				var p = renderPoints[i];
				mesh.SetNextVertex(new Vertex
				{
					position = new Vector3(p.x, p.y, Vertex.nearZ),
				});
				mesh.SetNextVertex(new Vertex
				{
					position = new Vector3(p.x, p.y, Vertex.nearZ),
					tint = color
				});
			}
		}

		//
		// void DrawEdge(MeshGenerationContext mgc)
		//    {
		//        if (edgeWidth <= 0)
		//            return;
		//
		//        UpdateRenderPoints();
		//        if (m_RenderPoints.Count == 0)
		//            return; // Don't draw anything
		//
		//        Color inColor = this.inputColor;
		//        Color outColor = this.outputColor;
		//
		//        inColor *= UIElementsUtility.editorPlayModeTintColor;
		//        outColor *= UIElementsUtility.editorPlayModeTintColor;
		//
		//        uint cpt = (uint)m_RenderPoints.Count;
		//        uint wantedLength = (cpt) * 2;
		//        uint indexCount = (wantedLength - 2) * 3;
		//
		//        var md = mgc.Allocate((int)wantedLength, (int)indexCount, null, null, MeshGenerationContext.MeshFlags.UVisDisplacement);
		//        if (md.vertexCount == 0)
		//            return;
		//
		//        float polyLineLength = 0;
		//        for (int i = 1; i < cpt; ++i)
		//            polyLineLength += (m_RenderPoints[i - 1] - m_RenderPoints[i]).sqrMagnitude;
		//
		//        float halfWidth = edgeWidth * 0.5f;
		//        float currentLength = 0;
		//        Color32 flags = new Color32(0, 0, 0, (byte)VertexFlags.LastType);
		//
		//        Vector2 unitPreviousSegment = Vector2.zero;
		//        for (int i = 0; i < cpt; ++i)
		//        {
		//            Vector2 dir;
		//            Vector2 unitNextSegment = Vector2.zero;
		//            Vector2 nextSegment = Vector2.zero;
		//
		//            if (i < cpt - 1)
		//            {
		//                nextSegment = (m_RenderPoints[i + 1] - m_RenderPoints[i]);
		//                unitNextSegment = nextSegment.normalized;
		//            }
		//
		//
		//            if (i > 0 && i < cpt - 1)
		//            {
		//                dir = unitPreviousSegment + unitNextSegment;
		//                dir.Normalize();
		//            }
		//            else if (i > 0)
		//            {
		//                dir = unitPreviousSegment;
		//            }
		//            else
		//            {
		//                dir = unitNextSegment;
		//            }
		//
		//            Vector2 pos = m_RenderPoints[i];
		//            Vector2 uv = new Vector2(dir.y * halfWidth, -dir.x * halfWidth); // Normal scaled by half width
		//            Color32 tint = Color.LerpUnclamped(outColor, inColor, currentLength / polyLineLength);
		//
		//            md.SetNextVertex(new Vertex() { position = new Vector3(pos.x, pos.y, 1), uv = uv, tint = tint, idsFlags = flags });
		//            md.SetNextVertex(new Vertex() { position = new Vector3(pos.x, pos.y, -1), uv = uv, tint = tint, idsFlags = flags });
		//
		//            if (i < cpt - 2)
		//            {
		//                currentLength += nextSegment.sqrMagnitude;
		//            }
		//            else
		//            {
		//                currentLength = polyLineLength;
		//            }
		//
		//            unitPreviousSegment = unitNextSegment;
		//        }
		//
		//        // Fill triangle indices as it is a triangle strip
		//        for (uint i = 0; i < wantedLength - 2; ++i)
		//        {
		//            if ((i & 0x01) == 0)
		//            {
		//                md.SetNextIndex((UInt16)i);
		//                md.SetNextIndex((UInt16)(i + 2));
		//                md.SetNextIndex((UInt16)(i + 1));
		//            }
		//            else
		//            {
		//                md.SetNextIndex((UInt16)i);
		//                md.SetNextIndex((UInt16)(i + 1));
		//                md.SetNextIndex((UInt16)(i + 2));
		//            }
		//        }
		//    }
	}
}
