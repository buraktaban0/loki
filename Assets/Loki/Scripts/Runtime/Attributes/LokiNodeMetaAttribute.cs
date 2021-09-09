using System;

namespace Loki.Runtime.Attributes
{
	[AttributeUsage(AttributeTargets.Class, AllowMultiple = false, Inherited = false)]
	public class LokiNodeMetaAttribute : Attribute
	{
		private readonly string m_Path;
		public string Path => m_Path;

		public LokiNodeMetaAttribute(string path)
		{
			m_Path = path;
		}
	}
}
