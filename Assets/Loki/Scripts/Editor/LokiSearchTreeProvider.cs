using System.Collections.Generic;

namespace Loki.Editor
{
	public class LokiSearchTreeProvider
	{
		public LokiSearchEntry GetEntryTree()
		{
			var entries = new Dictionary<string, LokiSearchEntry>()
			{
				{
					"Test0", new LokiSearchEntry()
					{
						visibleName = "Test 0", isGroup = true, children = new Dictionary<string, LokiSearchEntry>()
						{
							{"Child0", new LokiSearchEntry() {visibleName = "Child 0"}},
							{"Child1", new LokiSearchEntry() {visibleName = "Child 1"}}
						}
					}
				},
				{"Test1", new LokiSearchEntry() {visibleName = "Test 1"}},
				{"Test2", new LokiSearchEntry() {visibleName = "Test 2"}},
			};

			var tree = new LokiSearchEntry() {children = entries, isGroup = true};

			return tree;
		}
	}
}
