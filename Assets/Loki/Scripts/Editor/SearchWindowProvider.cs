using System;
using System.Collections.Generic;
using UnityEditor.Experimental.GraphView;

namespace Loki.Scripts.Editor
{
	public class SearchWindowProvider : ISearchWindowProvider
	{
		
		
		public void Prepare()
		{
		}

		public List<SearchTreeEntry> CreateSearchTree(SearchWindowContext context)
		{
			var entries = new List<SearchTreeEntry>();

			
			
			return entries;
		}

		public bool OnSelectEntry(SearchTreeEntry SearchTreeEntry, SearchWindowContext context)
		{
			return false;
		}
	}
}
