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

		public LokiNodeView()
		{
			var visualTree = LokiResources.Get<VisualTreeAsset>("UXML/LokiNodeView.uxml");
			visualTree.CloneTree(this);

			var styleSheet = LokiResources.Get<StyleSheet>("StyleSheets/LokiNodeView.uss");
			styleSheets.Add(styleSheet);
			
			this.AddToClassList("node");
			
			capabilities |= Capabilities.Selectable | Capabilities.Movable | Capabilities.Deletable | Capabilities.Ascendable | Capabilities.Copiable;
			usageHints = UsageHints.DynamicTransform;

			style.position = Position.Absolute;

		}

		public void CollectElements(HashSet<GraphElement> collectedElementSet, Func<GraphElement, bool> conditionFunc)
		{
		}
	}
}
