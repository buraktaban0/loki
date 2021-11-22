using System;
using System.Reflection;
using UnityEngine;

namespace Loki.Runtime.Database
{
	[System.Serializable]
	public class SerializedMethodInfo : ISerializationCallbackReceiver
	{
		[SerializeField]
		private string m_Type;

		[SerializeField]
		private string m_Method;

		public MethodInfo Method;

		public void OnBeforeSerialize()
		{
			if (Method == null)
			{
				m_Type = string.Empty;
				m_Method = string.Empty;
				return;
			}

			m_Type = Method.DeclaringType.AssemblyQualifiedName;
			m_Method = Method.Name;
		}

		public void OnAfterDeserialize()
		{
			if (string.IsNullOrEmpty(m_Type) || string.IsNullOrEmpty(m_Method))
			{
				m_Type = string.Empty;
				m_Method = string.Empty;
				Method = null;
				return;
			}

			try
			{
				var type = Type.GetType(m_Type);
				Method = type.GetMethod(m_Method, BindingFlags.Static);
			}
			catch (Exception e)
			{
				Debug.LogException(new Exception("Failed to deserialize method info.", e));
			}
		}
	}
}
