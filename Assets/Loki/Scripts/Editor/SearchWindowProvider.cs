using System;
using System.Collections.Generic;
using UnityEditor.Experimental.GraphView;
using UnityEngine;

namespace Loki.Editor
{
	public class SearchWindowProvider : ScriptableObject, ISearchWindowProvider
	{
	
		public void Prepare()
		{
		}

		public List<SearchTreeEntry> CreateSearchTree(SearchWindowContext cxt)
		{
			var entries = new List<SearchTreeEntry>();


			return entries;
		}

		public bool OnSelectEntry(SearchTreeEntry entry, SearchWindowContext cxt)
		{
			return false;
		}
	}
}
