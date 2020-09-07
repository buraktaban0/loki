using System;
using System.Collections.Generic;
using System.Linq;

namespace Loki.Editor
{
	public class LokiSearchEntry
	{
		public LokiSearchEntry parent;

		public string visibleName = "";
		public string name;

		public object userData;

		public Dictionary<string, LokiSearchEntry> children = new Dictionary<string, LokiSearchEntry>();

		public bool isGroup;

		public bool hasChildren => children.Any();

		public void Add(LokiSearchEntry entry)
		{
			entry.parent = this;
			children.Add(entry.name, entry);
		}

		public void AddRange(IEnumerable<LokiSearchEntry> entries)
		{
			foreach (var entry in entries)
			{
				children.Add(entry.name, entry);
			}
		}
	}
}
