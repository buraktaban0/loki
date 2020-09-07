using System;
using System.Collections.Generic;
using System.Linq;
using Loki.Editor;
using Loki.Editor.Utility;
using UnityEditor;
using UnityEditor.Experimental.GraphView;
using UnityEngine;
using UnityEngine.UIElements;
using UnityEditor.UIElements;


public class LokiSearchWindow : EditorWindow
{
	[MenuItem("Loki/Search")]
	public static void ShowExample()
	{
		var wnd = ScriptableObject.CreateInstance<LokiSearchWindow>();
		wnd.Show();

		wnd.titleContent = new GUIContent("LokiSearchWindow".SplitCamelCase());

		var root = new LokiSearchEntry {isGroup = true, visibleName = "Test 1", name = "test0"};
		root.Add(new LokiSearchEntry {isGroup = false, visibleName = "Child", name = "child0"});

		wnd.entries = new Dictionary<string, LokiSearchEntry>
		              {
			              {root.name, root}
		              };

		wnd.Populate();
	}


	public Dictionary<string, LokiSearchEntry> entries;

	public void OnEnable()
	{
		var visualTree = LokiResources.Get<VisualTreeAsset>("UXML/LokiSearchWindow.uxml");
		visualTree.CloneTree(rootVisualElement);

		var ss = LokiResources.Get<StyleSheet>("StyleSheets/LokiSearchWindow.uss");
		rootVisualElement.styleSheets.Add(ss);


		this.Focus();
	}

	private void OnLostFocus()
	{
		//this.Close();
	}

	public void Populate()
	{
		var listView = rootVisualElement.Q<ListView>();
		listView.itemsSource = entries.Values.ToArray();
		listView.makeItem = () =>
		{
			var el = new VisualElement();
			el.Add(new Label());
			return el;
		};

		listView.bindItem = (element, i) => { element.Q<Label>().text = entries.Values.ToArray()[i].visibleName; };

		listView.Refresh();
	}
}
