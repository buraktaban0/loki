using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
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
		private List<SerializedMethodInfo> m_MethodDefinitions;

#if UNITY_EDITOR
		private void Awake()
		{
			if (m_MethodDefinitions == null || m_MethodDefinitions.Count < 1)
			{
				Initialize();
			}
		}

		private void Initialize()
		{
			m_MethodDefinitions = new List<SerializedMethodInfo>();
			foreach (var assembly in AppDomain.CurrentDomain.GetAssemblies())
			{
				foreach (var type in assembly.GetTypes())
				{
					foreach (var methodInfo in type.GetMethods(BindingFlags.Static)
					                               .Where(info => info.IsStatic && !info.IsGenericMethod &&
					                                              info.IsDefined(typeof(LokiAttribute))))
					{
						m_MethodDefinitions.Add(new SerializedMethodInfo
						                        {
							                        Method = methodInfo
						                        });
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
		}

		public Type GetRunnerTypeForGraphType(Type graphType)
		{
			return default;
		}
	}
}
