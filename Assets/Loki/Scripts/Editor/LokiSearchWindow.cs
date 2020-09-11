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

	private static readonly bool popup = false;

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

		if (popup)
			window.ShowPopup();
		else
			window.Show();

		window.Initialize();
	}

	private bool isMain = false;

	private bool canInteract = true;

	private LokiSearchTreeProvider provider;

	private TextField textField;
	private VisualElement textFieldFocusController;

	private Label labelInfo;

	private VisualElement pageContainer;

	private LokiSearchTree rootTree;

	private List<LokiSearchTree> currentItems;

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

		textFieldFocusController.RegisterCallback<KeyDownEvent>(evt =>
		{
			if (evt.keyCode == KeyCode.DownArrow)
			{
				var list = pageStack[currentPage];
				list.SetSelection(0);
				list.Focus();
			}
		});

		rootVisualElement.RegisterCallback<KeyDownEvent>(evt =>
		{
			if (evt.keyCode == KeyCode.LeftArrow)
			{
				if (currentPage == 0)
					return;

				MoveLeft();
			}
		});

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
			if (popup)
				this.Close();
		}
	}

	private void Initialize()
	{
		rootTree = provider.GetEntryTree();

		currentPage = 0;
		GetOrCreateList(rootTree, 0);

		textFieldFocusController.Focus();
	}


	private void MoveRight(LokiSearchTree tree)
	{
		if (!canInteract)
			return;

		GetOrCreateList(tree, ++currentPage);
		AnimatePageContainer(currentPage);
	}

	private void MoveLeft()
	{
		if (!canInteract || currentPage == 0)
			return;
		AnimatePageContainer(--currentPage);
	}


	private ListView GetOrCreateList(LokiSearchTree tree, int pageIndex)
	{
		if (pageIndex < pageStack.Count)
		{
			var existingList = pageStack[pageIndex];
			if (tree != null)
				BindList(existingList, tree);
			return existingList;
		}

		var listView = new ListView(null, ITEM_HEIGHT, MakeEntryElement, BindEntryElement);
		listView.AddToClassList("search-list");
		listView.showBorder = false;
		listView.style.width = size.x;
		listView.style.minWidth = size.x;
		listView.style.maxWidth = size.x;

		listView.pageContainer.Add(listView);
		pageStack.Add(listView);

		BindList(listView, tree);

		return listView;
	}

	private void BindList(ListView listView, LokiSearchTree tree)
	{
		listView.userData = tree;
		currentItems = tree.Flatten();
		listView.itemsSource = currentItems;

		listView.Refresh();
	}


	private void AnimatePageContainer(int pageIndex, int animDuration = 200)
	{
		Vector3 to = Vector3.left * (size.x * pageIndex);
		var anim = pageContainer.experimental.animation.Position(to, animDuration).Ease(Easing.OutQuad);

		canInteract = false;
		anim.OnCompleted(() => { canInteract = true; });

		textFieldFocusController.Focus();
	}


	private void OnSearchTextChanged(ChangeEvent<string> evt)
	{
		var text = evt.newValue.Trim();
		if (text == evt.previousValue)
		{
			return;
		}

		if (string.IsNullOrEmpty(text))
		{
			labelInfo.text = "";
			GetOrCreateList(rootTree, 0);
			AnimatePageContainer(0, 1);
			return;
		}

		text = text.ToLower();
		var keywords = text.Split(' ');

		var tree = rootTree.Clone();

		tree.Filter(keywords);

		GetOrCreateList(tree, 0);
		AnimatePageContainer(0, 1);

		//labelInfo.text = (count == 0 ? "No" : count.ToString()) + (count > 1 ? " results" : " result") + " found.";
	}

	private void OnClickEntry(MouseUpEvent evt)
	{
	}

	private void OnClickEntryGroup(MouseUpEvent evt)
	{
		var el = evt.target as VisualElement;
		var group = (LokiSearchTree) el.userData;

		currentPage++;
		GetOrCreateList(group, currentPage);

		AnimatePageContainer(currentPage);
	}

	private void BindEntryElement(VisualElement el, int i)
	{
		var entry = currentItems[i];
		el.userData = entry;

		var lbl = el.Q<Label>();
		lbl.text = entry.name;

		var lblCount = el.Q<Label>("label-count");

		var arrowImg = el.Q<Image>();

		el.UnregisterCallback<MouseUpEvent>(OnClickEntry);
		el.UnregisterCallback<MouseUpEvent>(OnClickEntryGroup);

		if (entry.isGroup)
		{
			lblCount.text = entry.matchCount.ToString();

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
			lblCount.text = "";

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

		var lblCount = new Label {pickingMode = PickingMode.Ignore, name = "label-count"};
		lbl.AddToClassList("list-item-label");
		lblCount.style.right = 0f;
		lblCount.style.unityTextAlign = TextAnchor.MiddleRight;
		lblCount.style.marginRight = 8;
		el.Add(lblCount);

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
