using System;
using System.Collections.Generic;
using System.Linq;
using Loki.Editor.Utility;

namespace Loki.Editor
{
	public struct LokiSearchQueryEntry
	{
		public LokiSearchTree tree;
		public int matchCount;
	}

	public class LokiSearchTree
	{
		private static readonly Comparison<LokiSearchTree> comparison = (a, b) =>
			b.matchCount - a.matchCount;

		public LokiSearchTree parent;

		public List<LokiSearchTree> children = new List<LokiSearchTree>();

		public int childCount => children.Count;

		public string name;

		public object userData;


		public bool isGroup;

		public int matchCount = -1;

		public LokiSearchTree(bool isGroup = false)
		{
			this.isGroup = isGroup;
		}

		public LokiSearchTree(List<LokiSearchTree> children)
		{
			isGroup = true;
			this.children = children;
		}


		public override string ToString()
		{
			return name.SplitCamelCase();
		}

		public void Add(LokiSearchTree tree)
		{
			tree.parent = this;
			children.Add(tree);
		}

		public void AddRange(IEnumerable<LokiSearchTree> entries)
		{
			foreach (var entry in entries)
			{
				entry.parent = this;
				children.Add(entry);
			}
		}

		public List<LokiSearchTree> Flatten()
		{
			var list = new List<LokiSearchTree>();

			for (int i = 0; i < childCount; i++)
			{
				var entry = children[i];
				var visibleEntry = entry.GetVisibleEntry();
				if (visibleEntry != null)
					list.Add(visibleEntry);
			}

			return list;
		}

		public LokiSearchTree GetVisibleEntry()
		{
			if (isGroup && matchCount == 1)
			{
				return children.First(c => c.matchCount > 0);
			}

			if (matchCount == -1 || matchCount > 0)
				return this;

			return null;
		}


		public LokiSearchTree Clone()
		{
			var tree = new LokiSearchTree(isGroup);
			if (isGroup)
			{
				var clonedChildren = children.Select(c => c.Clone());
				tree.AddRange(clonedChildren);
			}

			tree.name = this.name;
			tree.userData = this.userData;

			return tree;
		}

		public void Filter(IEnumerable<string> keywords)
		{
			var kwList = keywords.ToList();

			// Add the whole word too, bringing complete matches to the top by match count.
			kwList.Add(string.Join(" ", kwList));


			var trie = new Trie();
			trie.Add(kwList);
			trie.Build();

			this.Filter(trie);
		}

		private void Filter(Trie trie)
		{
			if (isGroup)
			{
				for (var i = children.Count - 1; i >= 0; i--)
				{
					var entry = children[i];
					entry.Filter(trie);

					if (entry.matchCount < 1)
					{
						children.RemoveAt(i);
					}
				}

				this.matchCount = children.Count;
				children.Sort(comparison);
			}
			else
			{
				var matches = trie.Find(name.ToLower());
				this.matchCount = matches.Count();
			}
		}
	}
}
