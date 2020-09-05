using System;
using System.Collections.Generic;
using Loki.Editor.Utility;
using UnityEditor;
using UnityEditor.Experimental.GraphView;
using UnityEngine;
using UnityEngine.UIElements;

namespace Loki.Editor
{
	public class LokiNodeView : GraphElement, ICollectibleElement
	{
		public static LokiNodeView Get(LokiNode node)
		{
			var nodeView = new LokiNodeView(0);

			return nodeView;
		}


		private const string SELECTED_CLASS_NAME = "selected";
		private const string HOVER_CLASS_NAME = "hover";

		private readonly VisualElement container;
		private readonly VisualElement selectionBorder;

		private readonly VisualElement headerContainer;
		private readonly VisualElement midContainer;

		public string guid = Guid.NewGuid().ToString();

		public LokiNodeView(int i)
		{
			var visualTree = LokiResources.Get<VisualTreeAsset>("UXML/LokiNodeView.uxml");
			visualTree.CloneTree(this);

			var styleSheet = LokiResources.Get<StyleSheet>("StyleSheets/LokiNodeView.uss");
			styleSheets.Add(styleSheet);

			this.AddToClassList("node");

			capabilities |= Capabilities.Selectable | Capabilities.Movable | Capabilities.Deletable |
			                Capabilities.Ascendable | Capabilities.Copiable;
			usageHints = UsageHints.DynamicTransform;

			style.position = Position.Absolute;

			selectionBorder = this.Q<VisualElement>("selection-border");
			container = this.Q<VisualElement>("node-root");

			headerContainer = this.Q<VisualElement>("header-container");
			midContainer = this.Q<VisualElement>("mid-container");


			var port = new LokiPort(Orientation.Horizontal, i == 0 ? Direction.Input : Direction.Output,
			                        Capacity.Single);
			headerContainer.Insert(i, port);

			container.RegisterCallback<GeometryChangedEvent>(OnRootGeometryChanged);

			this.RegisterCallback<MouseEnterEvent>(OnMouseEnter);
			this.RegisterCallback<MouseLeaveEvent>(OnMouseLeave);
		}

		private void OnMouseEnter(MouseEnterEvent evt)
		{
			selectionBorder.AddToClassList(LokiEditorUtility.CLASS_HOVER);
		}

		private void OnMouseLeave(MouseLeaveEvent evt)
		{
			selectionBorder.RemoveFromClassList(LokiEditorUtility.CLASS_HOVER);
		}

		private void OnRootGeometryChanged(GeometryChangedEvent evt)
		{
			selectionBorder.pickingMode = PickingMode.Ignore;
			selectionBorder.style.position = Position.Absolute;
			selectionBorder.style.width = container.layout.width;
			selectionBorder.style.height = container.layout.height;
		}

		public override void OnSelected()
		{
			base.OnSelected();
			selectionBorder.AddToClassList(LokiEditorUtility.CLASS_SELECTED);
			//selectionBorder.RemoveFromClassList(UNSELECTED_CLASS_NAME);
		}

		public override void OnUnselected()
		{
			base.OnUnselected();
			selectionBorder.RemoveFromClassList(LokiEditorUtility.CLASS_SELECTED);
//			selectionBorder.AddToClassList(UNSELECTED_CLASS_NAME);
		}

		public void SetPosition(Vector2 position)
		{
			this.style.position = Position.Absolute;
			this.style.top = position.y;
			this.style.left = position.x;
		}

		public override void Select(VisualElement selectionContainer, bool additive)
		{
			base.Select(selectionContainer, additive);
		}

		public void CollectElements(HashSet<GraphElement> collectedElementSet, Func<GraphElement, bool> conditionFunc)
		{
			collectedElementSet.Add(this);
		}
	}
}
