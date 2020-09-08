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
using UnityEngine.UIElements.Experimental;


public class LokiSearchWindow : EditorWindow
{
	private static readonly Vector2 size = new Vector2(300, 450);

	private static void ClosePresentWindows()
	{
		GetWindow<LokiSearchWindow>().Close();
	}

	public static void Open(LokiSearchTreeProvider provider, Vector2 position)
	{
		ClosePresentWindows();

		var window = ScriptableObject.CreateInstance<LokiSearchWindow>();
		window.titleContent = new GUIContent("Loki Search");

		position.x -= size.x / 2;

		var rect = new Rect(position, size);

		window.position = rect;
		window.minSize = size;
		window.maxSize = size;

		window.isMain = true;

		window.provider = provider;

		window.ShowPopup();

		window.Initialize();
	}

	private bool isMain = false;

	private bool canInteract = true;

	private LokiSearchTreeProvider provider;

	private TextField textField;
	private VisualElement textFieldFocusController;

	private Label labelInfo;

	private VisualElement pageContainer;

	private VisualElement page0Container;
	private VisualElement page1Container;

	private ListView list0;
	private ListView list1;

	private LokiSearchEntry rootEntry;
	private LokiSearchEntry currentRootEntry;
	public Dictionary<string, LokiSearchEntry> entries;

	private LokiSearchEntry[] values0;

	public void OnEnable()
	{
		var vt = LokiResources.Get<VisualTreeAsset>("UXML/LokiSearchWindow.uxml");
		vt.CloneTree(rootVisualElement);

		var ss = LokiResources.Get<StyleSheet>("StyleSheets/LokiSearchWindow.uss");
		rootVisualElement.styleSheets.Add(ss);

		labelInfo = rootVisualElement.Q<Label>("label-info");

		textField = rootVisualElement.Q<TextField>();
		textFieldFocusController = textField.Q("unity-text-input");
		textFieldFocusController.Focus();

		rootVisualElement.RegisterCallback<KeyDownEvent>(OnEscapeKeyDown);
		textFieldFocusController.RegisterCallback<KeyDownEvent>(OnEscapeKeyDown);

		textField.RegisterCallback<ChangeEvent<string>>(OnSearchTextChanged);

		pageContainer = rootVisualElement.Q("page-container");
		page0Container = rootVisualElement.Q("page0-container");
		page1Container = rootVisualElement.Q("page1-container");

		page0Container.style.width = size.x;
		page0Container.style.minWidth = size.x;
		page0Container.style.maxWidth = size.x;

		page1Container.style.width = size.x;
		page1Container.style.minWidth = size.x;
		page1Container.style.maxWidth = size.x;

		list0 = new ListView(null, 20, MakeEntryElement, BindEntryElement);
		list1 = new ListView(null, 20, MakeEntryElement, BindEntryElement);

		list0.AddToClassList("search-list");
		list1.AddToClassList("search-list");

		page0Container.Add(list0);
		page1Container.Add(list1);


		this.Focus();
	}

	private void OnLostFocus()
	{
		if (isMain)
		{
			this.Close();
		}
	}

	private void Initialize()
	{
		var treeRoot = provider.GetEntryTree();
		rootEntry = treeRoot;

		currentRootEntry = treeRoot;
		entries = treeRoot.children;

		SetupPage(list0, treeRoot);

		textFieldFocusController.Focus();
	}


	private void AnimatePageContainer(int i)
	{
		Vector3 pos = i == 0 ? Vector3.zero : new Vector3(-size.x, 0f, 0f);

		int duration = 300; // ms
		var anim = pageContainer.experimental.animation.Position(pos, duration).Ease(Easing.OutQuad);

		canInteract = false;
		anim.OnCompleted(() => { canInteract = true; });

		textFieldFocusController.Focus();
	}

	private void SetupPage(ListView page, LokiSearchEntry root)
	{
		var items = root.children.Values.ToArray();
		items = items.OrderBy(entry => entry.isGroup ? 0 : 1).ToArray();


		page.itemsSource = items;
		page.makeItem = MakeEntryElement;
		page.bindItem = BindEntryElement;
		page.itemHeight = 20;
		page.showBorder = true;

		values0 = items;

		page.Refresh();

		textFieldFocusController.Focus();
	}


	private void OnSearchTextChanged(ChangeEvent<string> evt)
	{
		var text = evt.newValue;

		if (string.IsNullOrEmpty(text))
			return;

		text = text.Trim();
		var keywords = text.Split(' ');

		var a = string.Join("_", keywords);

		
		var searchResults = rootEntry.Query(keywords);

		Debug.Log(a + " - " + searchResults.Count + " results.");
	}

	private void OnClickEntry(MouseUpEvent evt)
	{
		Debug.Log("entry");
	}

	private void OnClickEntryGroup(MouseUpEvent evt)
	{
		var el = evt.target as VisualElement;
		var group = (LokiSearchEntry) el.userData;

		SetupPage(list1, group);
		AnimatePageContainer(1);

		Debug.Log("group " + group.visibleName);
	}

	private void BindEntryElement(VisualElement el, int i)
	{
		var entry = values0[i];

		el.userData = entry;

		var lbl = el.Q<Label>();
		lbl.text = entry.visibleName;

		var arrowImg = el.Q<Image>();

		el.UnregisterCallback<MouseUpEvent>(OnClickEntry);
		el.UnregisterCallback<MouseUpEvent>(OnClickEntryGroup);

		if (entry.isGroup)
		{
			if (arrowImg == null)
			{
				arrowImg = new Image();
				arrowImg.pickingMode = PickingMode.Ignore;
				arrowImg.AddToClassList("group-entry-icon");
				el.Insert(0, arrowImg);
			}

			el.AddToClassList("no-padding");
			el.RegisterCallback<MouseUpEvent>(OnClickEntryGroup);
		}
		else
		{
			el.RemoveFromClassList("no-padding");
			arrowImg?.RemoveFromHierarchy();

			el.RegisterCallback<MouseUpEvent>(OnClickEntry);
		}
	}


	private VisualElement MakeEntryElement()
	{
		var el = new VisualElement();
		el.AddToClassList("list-item");
		var lbl = new Label();
		lbl.pickingMode = PickingMode.Ignore;
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
}
