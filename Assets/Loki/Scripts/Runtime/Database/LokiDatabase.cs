using System;
using System.Collections.Generic;
using System.Linq;
using Loki.Runtime.Attributes;
using Loki.Runtime.Core;
using Loki.Utility;
#if UNITY_EDITOR
using UnityEditor;
#endif
using UnityEngine;

namespace Loki.Runtime.Database
{
	public class LokiDatabase : ScriptableObject, ISerializationCallbackReceiver //, IPreprocessBuildWithReport
	{
		// public int callbackOrder => 0;
		// public void OnPreprocessBuild(BuildReport report)
		// {
		// 	if (LokiSettings.Get().RebuildDatabaseBeforeBuild == false)
		// 		return;
		//
		// 	Reinitialize();
		// }

		[UnityEditor.Callbacks.DidReloadScripts]
		public static void OnAfterReloadDomain()
		{
			if (LokiSettings.Get().RebuildDatabaseAfterCompilation == false)
				return;

			Reinitialize();
		}

		[MenuItem("Loki/Reset Database")]
		private static void Reinitialize()
		{
			EditorApplication.delayCall += () =>
			{
				var isCreated = IsCreated();
				var db = Get();

				if (isCreated) // Was already created, Get() will not re-initialize the database.
				{
					db.Initialize();
				}
			};
		}

		[MenuItem("Loki/Test Database")]
		private static void TestDB()
		{
			EditorApplication.delayCall += () =>
			{
				var db = Get();
				Debug.Log(JsonUtility.ToJson(db, true));
			};
		}

		public static bool IsCreated()
		{
			return AssetDatabaseHelpers.LoadAll<LokiDatabase>().Any();
		}

		public static LokiDatabase Get()
		{
			var dbs = AssetDatabaseHelpers.LoadAll<LokiDatabase>();
			var db = dbs.FirstOrDefault();
			if (db == null)
			{
				db = CreateInstance<LokiDatabase>();
				AssetDatabaseHelpers.CreateAssetMkdir(db, "Assets/Loki/LokiDatabase.asset");
			}
			else
			{
				for (var i = 1; i < dbs.Count; i++)
				{
					DestroyImmediate(dbs[i], true);
				}

				AssetDatabase.SaveAssets();
			}

			return db;
		}


		[SerializeField]
		private List<LokiNodeDefinition> m_NodeDefinitions;

		private Dictionary<Type, LokiNodeDefinition> m_NodeDefinitionsByType;

#if UNITY_EDITOR
		private void Awake()
		{
			if (m_NodeDefinitions == null || m_NodeDefinitions.Count < 1)
			{
				Initialize();
			}
		}

		private void Initialize()
		{
			m_NodeDefinitions = new List<LokiNodeDefinition>();
			var lokiNodeType = typeof(LokiNode);
			var nodeAttrType = typeof(LokiNodeMetaAttribute);
			foreach (var assembly in AppDomain.CurrentDomain.GetAssemblies())
			{
				foreach (var type in assembly.GetTypes())
				{
					if (!type.IsAbstract && type.IsSubclassOf(lokiNodeType) && type.IsDefined(nodeAttrType, false))
					{
						var def = LokiNodeDefinition.FromType(type);
						m_NodeDefinitions.Add(def);
					}
				}
			}

			EditorUtility.SetDirty(this);
			AssetDatabase.SaveAssets();
		}
#endif

		public void OnBeforeSerialize()
		{
		}

		public void OnAfterDeserialize()
		{
			m_NodeDefinitionsByType = m_NodeDefinitions.ToDictionary(def => def.Type);
		}

		public LokiNodeDefinition GetDefinition(Type nodeType)
		{
			if (m_NodeDefinitionsByType.TryGetValue(nodeType, out var def))
			{
				return def;
			}

			throw new Exception($"Node type is not defined in the database: {nodeType.FullName}");
		}
	}
}
