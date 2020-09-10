using System.Collections.Generic;

namespace Loki.Editor
{
	public class LokiSearchTreeProvider
	{
		public LokiSearchTree GetEntryTree()
		{
			var c0 = new LokiSearchTree() {name = "Burak Taban"};
			var c1 = new LokiSearchTree() {name = "Can Yılmaz"};
			var c2 = new LokiSearchTree() {name = "Uzay Doruk"};

			var g0 = new LokiSearchTree(new List<LokiSearchTree>
			{
				c0, c1, c2
			}) {name = "Test Group"};

			var e0 = new LokiSearchTree() {name = "Loki Test Root Entry"};
			var e1 = new LokiSearchTree() {name = "Loki Test Root Entry 2"};

			return new LokiSearchTree(new List<LokiSearchTree>
			{
				g0, e0, e1
			});
		}
	}
}
