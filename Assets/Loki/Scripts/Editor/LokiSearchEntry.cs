using System;
using System.Collections.Generic;
using System.Linq;
using Loki.Editor.Utility;

namespace Loki.Editor
{
	public struct LokiSearchQueryEntry
	{
		public LokiSearchEntry entry;
		public int matchCount;
	}

	public class LokiSearchEntry
	{
		public LokiSearchEntry parent;

		public string name;

		public object userData;

		public List<LokiSearchEntry> children = new List<LokiSearchEntry>();

		public bool isGroup => children.Count > 0;

		public void Add(LokiSearchEntry entry)
		{
			entry.parent = this;
			children.Add(entry);
		}

		public void AddRange(IEnumerable<LokiSearchEntry> entries)
		{
			foreach (var entry in entries)
			{
				entry.parent = this;
				children.Add(entry);
			}
		}

		public List<LokiSearchEntry> Query(IEnumerable<string> keywords)
		{
			var kwList = keywords.ToList();

			// Add the whole word too, bringing complete matches to the top by match count.
			kwList.Add(string.Join(" ", kwList));

			var trie = new Trie();
			trie.Add(kwList);
			trie.Build();

			return this.Query(trie, new List<LokiSearchQueryEntry>()).OrderByDescending(q => q.matchCount)
			           .Select(q => q.entry).ToList();
		}

		private List<LokiSearchQueryEntry> Query(Trie trie, List<LokiSearchQueryEntry> entries)
		{
			if (isGroup)
			{
				for (var index = 0; index < children.Count; index++)
				{
					var entry = children[index];
					entry.Query(trie, entries);
				}
			}
			else
			{
				var matches = trie.Find(name);
				int matchCount = matches.Count();
				if (matchCount > 0)
				{
					entries.Add(new LokiSearchQueryEntry
					            {
						            entry = this,
						            matchCount = matchCount
					            });
				}
			}


			return entries;
		}
	}
}
