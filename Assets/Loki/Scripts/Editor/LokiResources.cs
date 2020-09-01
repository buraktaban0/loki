using System.Collections.Generic;
using System.Linq;
using UnityEditor;
using UnityEngine;

namespace Loki.Editor
{
	public static class LokiResources
	{
		private static readonly string ResourcePath = "Assets/Loki/Res";

		static LokiResources()
		{
		}

		public static T Get<T>(string name) where T : UnityEngine.Object
		{
			return (T) (AssetDatabase.LoadAssetAtPath<UnityEngine.Object>(ResourcePath + "/" + name));
		}


		public static string GetPath(string name)
		{
			if (name.StartsWith("/"))
				name = name.Substring(1, name.Length - 1);
			return $"{ResourcePath}/{name}";
		}
	}
}
