using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using Loki.Runtime.Attributes;
using UnityEngine;

namespace Loki.Runtime.Core
{
	[Serializable]
	public class LokiNodeDefinition : ISerializationCallbackReceiver
	{
		[SerializeField]
		private string m_TypeName;

		private Type m_Type;
		public Type Type => m_Type;

		[SerializeField]
		private string m_Path;

		public string Path => m_Path;

		public List<LokiFieldDefinition> InputDefinitions;
		public List<LokiFieldDefinition> OutputDefinitions;

		public static LokiNodeDefinition FromType(Type type)
		{
			var attr = type.GetCustomAttribute(typeof(LokiNodeMetaAttribute)) as LokiNodeMetaAttribute;
			var def = new LokiNodeDefinition();
			def.m_Type = type;
			def.m_Path = attr.Path;

			def.InputDefinitions = new List<LokiFieldDefinition>();
			def.OutputDefinitions = new List<LokiFieldDefinition>();

			foreach (var inputField in type.GetFields(BindingFlags.Public | BindingFlags.Instance)
			                               .Where(field => field.IsDefined(typeof(InputAttribute))))
			{
				var inputAttr = inputField.GetCustomAttribute(typeof(InputAttribute));
				def.InputDefinitions.Add(new LokiFieldDefinition
					                         {Name = inputField.Name, Type = inputField.FieldType});
			}

			foreach (var outputField in type.GetFields(BindingFlags.Public | BindingFlags.Instance)
			                                .Where(field => field.IsDefined(typeof(OutputAttribute))))
			{
				var outputAttr = outputField.GetCustomAttribute(typeof(OutputAttribute));
				def.OutputDefinitions.Add(new LokiFieldDefinition
					                          {Name = outputField.Name, Type = outputField.FieldType});
			}

			return def;
		}

		public void OnBeforeSerialize()
		{
			m_TypeName = m_Type.AssemblyQualifiedName;
		}

		public void OnAfterDeserialize()
		{
			m_Type = System.Type.GetType(m_TypeName);
		}
	}
}
