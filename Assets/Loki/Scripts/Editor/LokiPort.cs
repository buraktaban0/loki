using System;
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
		private const string HOVER_CLASS_NAME = "hover";

		private readonly VisualElement capBorder;
		private readonly VisualElement cap;

		private readonly VisualElement connectionPoint;

		public override bool canGrabFocus => true;

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

		public LokiPort(Orientation portOrientation, Direction portDirection, Capacity portCapacity)
		{
			var vst = LokiResources.Get<VisualTreeAsset>("UXML/LokiPort.uxml");
			var ss = LokiResources.Get<StyleSheet>("StyleSheets/LokiPort.uss");

			vst.CloneTree(this);

			this.styleSheets.Add(ss);

			this.name = "Dummy Port";

			this.pickingMode = PickingMode.Position;

			directionVec = portDirection == Direction.Input ? Vector3.left : Vector3.right;

			focusable = true;

			connectionPoint = this.Q<VisualElement>("connection-point");

			capBorder = this.Q<VisualElement>("outline-border");
			cap = this.Q<VisualElement>("cap");

			connectionPoint.RegisterCallback<MouseEnterEvent>(OnMouseEnterConnection);
			connectionPoint.RegisterCallback<MouseLeaveEvent>(OnMouseLeaveConnection);

			connectionPoint.RegisterCallback<MouseDownEvent>(OnMouseDownConnection);
		}


		protected virtual void OnMouseEnterConnection(MouseEnterEvent evt)
		{
			cap.AddToClassList(HOVER_CLASS_NAME);
			capBorder.AddToClassList(HOVER_CLASS_NAME);
		}

		protected virtual void OnMouseLeaveConnection(MouseLeaveEvent evt)
		{
			cap.RemoveFromClassList(HOVER_CLASS_NAME);
			capBorder.RemoveFromClassList(HOVER_CLASS_NAME);
		}

		protected virtual void OnMouseDownConnection(MouseDownEvent evt)
		{
			evt.StopImmediatePropagation();
		}


		public void SetColor(Color color)
		{
			capBorder.style.color = color;
			cap.style.backgroundColor = color;
		}
	}
}
