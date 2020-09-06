using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using Loki.Editor.Utility;
using UnityEditor.Experimental.GraphView;
using UnityEngine;

namespace Loki.Editor
{
	public class Search : SearchWindow
	{
	}

	public class SearchWindowProvider : ScriptableObject, ISearchWindowProvider
	{
		public LokiPort fromPort;

		private List<MethodInfo> methods;

		private List<Type> types;


		private Dictionary<string, List<MethodInfo>> methodsByNamespace = new Dictionary<string, List<MethodInfo>>();

		public void Prepare()
		{
			methods = new List<MethodInfo>(256);
			types = new List<Type>();

			foreach (var assembly in AppDomain.CurrentDomain.GetAssemblies())
			{
				foreach (var type in assembly.GetTypes())
				{
					var ns = type.Namespace;
					if (ns == null)
						continue;

					var typeMethods = type.GetMethods(BindingFlags.Public | BindingFlags.Static);

					if (typeMethods.Length < 1)
						continue;

					List<MethodInfo> ms;
					if (methodsByNamespace.TryGetValue(ns, out ms) == false)
					{
						ms = new List<MethodInfo>();
						methodsByNamespace[ns] = ms;
					}

					ms.AddRange(typeMethods);
				}
			}
		}

		public List<SearchTreeEntry> CreateSearchTree(SearchWindowContext cxt)
		{
			var entries = new List<SearchTreeEntry>
			{
				new SearchTreeGroupEntry(new GUIContent("Create Nodes"), level: 0),
			};

			List<string> groups = new List<string>();
			foreach (var key in methodsByNamespace.Keys.OrderBy(key => key))
			{
				var segments = key.Split('.');
				if (segments.Length == 0)
					segments = new[] {key};
				for (int i = 0; i < segments.Length; i++)
				{
					if (i == groups.Count || groups[i] != segments[i])
					{
						if (i == groups.Count)
							groups.Add(segments[i]);
						else
						{
							groups[i] = segments[i];
						}

						entries.Add(new SearchTreeGroupEntry(new GUIContent(segments[i]), i + 1));
					}
				}

				entries.AddRange(methodsByNamespace[key]
					                 .Select(m => new SearchTreeEntry(
							                         new GUIContent($"{m.DeclaringType.Name}.{m.Name}"))
						                         {level = segments.Length + 1}));
			}

			return entries;
		}

		public bool OnSelectEntry(SearchTreeEntry entry, SearchWindowContext cxt)
		{
			return false;
		}
	}
}
