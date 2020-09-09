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

	private static readonly int ITEM_HEIGHT = 20;

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

	private LokiSearchEntry rootEntry;
	

	private List<ListView> pageStack = new List<ListView>(8);

	private int currentPage = 0;

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


		textFieldFocusController.Focus();
	}


	private ListView GetOrCreateList(LokiSearchEntry entry, int pageIndex)
	{
		if (pageIndex < pageStack.Count)
		{
			var existingList = pageStack[pageIndex];
			BindList(existingList, entry);
			return existingList;
		}

		var listView = new ListView(null, ITEM_HEIGHT, MakeEntryElement, BindEntryElement);
		listView.AddToClassList("search-list");
		listView.showBorder = false;

		pageContainer.Add(listView);
		pageStack.Add(listView);

		BindList(listView, entry);

		return listView;
	}

	private void BindList(ListView listView, LokiSearchEntry entry)
	{
		listView.userData = entry;

		listView.Refresh();
	}


	private void AnimatePageContainer(int pageIndex)
	{
		Vector3 to = Vector3.left * (size.x * pageIndex);
		int duration = 200; // ms
		var anim = pageContainer.experimental.animation.Position(to, duration).Ease(Easing.OutQuad);

		canInteract = false;
		anim.OnCompleted(() => { canInteract = true; });

		textFieldFocusController.Focus();
	}


	private void OnSearchTextChanged(ChangeEvent<string> evt)
	{
		var text = evt.newValue;

		if (string.IsNullOrEmpty(text))
		{
			labelInfo.text = "";
			return;
		}

		text = text.Trim();
		var keywords = text.Split(' ');

		var a = string.Join("_", keywords);


		var searchResults = rootEntry.Query(keywords);
		int count = searchResults.Count;

		Debug.Log(a + " - " + searchResults.Count + " results.");

		labelInfo.text = (count == 0 ? "No" : count.ToString()) + (count > 1 ? " results" : " result") + " found.";
	}

	private void OnClickEntry(MouseUpEvent evt)
	{
		Debug.Log("entry");
	}

	private void OnClickEntryGroup(MouseUpEvent evt)
	{
		var el = evt.target as VisualElement;
		var group = (LokiSearchEntry) el.userData;


		AnimatePageContainer(1);

		Debug.Log("group " + group.name);
	}

	private void BindEntryElement(VisualElement el, int i)
	{
		var entry = values0[i];

		el.userData = entry;

		var lbl = el.Q<Label>();
		lbl.text = entry.name;

		var arrowImg = el.Q<Image>();

		el.UnregisterCallback<MouseUpEvent>(OnClickEntry);
		el.UnregisterCallback<MouseUpEvent>(OnClickEntryGroup);

		if (entry.isGroup)
		{
			if (arrowImg == null)
			{
				arrowImg = new Image {pickingMode = PickingMode.Ignore};
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

		var lbl = new Label {pickingMode = PickingMode.Ignore};
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
