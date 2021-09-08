using System;

namespace Loki.Runtime.Attributes
{
	[AttributeUsage(AttributeTargets.Class, AllowMultiple = false, Inherited = false)]
	public class NodeMetaAttribute : Attribute
	{
		private readonly string m_Path;
		public string Path => m_Path;

		public NodeMetaAttribute(string path)
		{
			m_Path = path;
		}
	}
}
