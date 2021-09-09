using System;
using UnityEngine;

namespace Loki.Runtime.Core
{
	[Serializable]
	public class LokiFieldDefinition : ISerializationCallbackReceiver
	{
		[SerializeField]
		private string m_TypeName;
		
		public Type Type { get; set; }
		
		[SerializeField]
		private string m_Name;

		public string Name
		{
			get => m_Name;
			set => m_Name = value;
		}

		public void OnBeforeSerialize()
		{
			m_TypeName = Type.AssemblyQualifiedName;
		}

		public void OnAfterDeserialize()
		{
			Type = System.Type.GetType(m_TypeName);
		}
	}
}
