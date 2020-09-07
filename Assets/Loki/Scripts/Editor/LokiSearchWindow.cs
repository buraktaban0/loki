using System;
using System.Collections;
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
	private static void ClosePresentWindows()
	{
		GetWindow<LokiSearchWindow>().Close();
	}

	public static void Open(LokiSearchTreeProvider provider, Vector2 position)
	{
		ClosePresentWindows();

		var window = ScriptableObject.CreateInstance<LokiSearchWindow>();
		window.titleContent = new GUIContent("Loki Search");

		var size = new Vector2(300, 450);
		var rect = new Rect(position, size);

		window.minSize = size;
		window.maxSize = size;
		window.position = rect;

		window.isMain = true;

		window.provider = provider;

		window.Show();

		window.Initialize();
	}

	private bool isMain = false;

	private LokiSearchTreeProvider provider;

	private TextField textField;
	private VisualElement textFieldFocusController;

	private VisualElement pageContainer;

	private ListView page0;
	private ListView page1;

	public Dictionary<string, LokiSearchEntry> entries;


	public void OnEnable()
	{
		var vt = LokiResources.Get<VisualTreeAsset>("UXML/LokiSearchWindow.uxml");
		vt.CloneTree(rootVisualElement);

		var ss = LokiResources.Get<StyleSheet>("StyleSheets/LokiSearchWindow.uss");
		rootVisualElement.styleSheets.Add(ss);

		textField = rootVisualElement.Q<TextField>();
		textFieldFocusController = textField.Q("unity-text-input");
		textFieldFocusController.Focus();

		pageContainer = rootVisualElement.Q("page-container");

		rootVisualElement.RegisterCallback<KeyDownEvent>(OnEscapeKeyDown);
		textFieldFocusController.RegisterCallback<KeyDownEvent>(OnEscapeKeyDown);

		page0 = new ListView();
		page1 = new ListView();

		pageContainer.Add(page0);
		pageContainer.Add(page1);



		page0.style.left = 0;
		page0.style.width = maxSize.x;

		page0.style.backgroundColor = Color.red;

		page1.style.left = maxSize.x / 2;
		page1.style.width = maxSize.x;

		page1.style.color = Color.green;

		this.Focus();
	}

	private void Initialize()
	{
		var tree = provider.GetEntryTree();
	}

	private void SetupPage(ListView page, IList items)
	{
		page.itemsSource = items;

		page.Refresh();
	}

	private void BindEntryElement(VisualElement el, int i)
	{
		var lbl = el.Q<Label>();
		lbl.text = entries.Values.ToArray()[i].visibleName;
	}

	private VisualElement MakeEntryElement()
	{
		var el = new VisualElement();
		el.AddToClassList("list-item");
		var lbl = new Label();
		lbl.AddToClassList("list-item-label");
		el.Add(lbl);
		return el;
	}

	private void OnEscapeKeyDown(KeyDownEvent evt)
	{
		if (evt.keyCode == KeyCode.Escape)
		{
			this.Close();
		}
	}

	private void OnLostFocus()
	{
		if (isMain)
		{
			//this.Close();
		}
	}
}
