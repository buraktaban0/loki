using System;
using System.Collections.Generic;
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
			var nodeView = new LokiNodeView();

			return nodeView;
		}


		private const string UNSELECTED_CLASS_NAME = "unselected";

		private VisualElement container;
		private VisualElement selectionBorder;


		public LokiNodeView()
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

			container.RegisterCallback<GeometryChangedEvent>(OnRootGeometryChanged);
		}

		private void OnRootGeometryChanged(GeometryChangedEvent evt)
		{
			selectionBorder.style.position = Position.Absolute;
			selectionBorder.style.width = container.layout.width;
			selectionBorder.style.height = container.layout.height;
		}

		public override void OnSelected()
		{
			base.OnSelected();
			selectionBorder.RemoveFromClassList(UNSELECTED_CLASS_NAME);
		}

		public override void OnUnselected()
		{
			base.OnUnselected();
			selectionBorder.AddToClassList(UNSELECTED_CLASS_NAME);
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
