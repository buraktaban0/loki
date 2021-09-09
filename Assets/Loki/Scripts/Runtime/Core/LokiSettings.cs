using System.Linq;
using Loki.Utility;
using UnityEngine;

namespace Loki.Runtime.Core
{
	public class LokiSettings : ScriptableObject
	{

			public static LokiSettings Get()
			{
				var settings = AssetDatabaseHelpers.LoadAll<LokiSettings>().FirstOrDefault();
				if (settings == null)
				{
					settings = CreateInstance<LokiSettings>();
					AssetDatabaseHelpers.CreateAssetMkdir(settings, "Assets/Loki/LokiSettings.asset");
				}

				return settings;
			}

		public bool RebuildDatabaseAfterCompilation = true;
		

	}
}
