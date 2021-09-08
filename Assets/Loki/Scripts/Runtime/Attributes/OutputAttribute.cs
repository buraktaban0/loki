using System;

namespace Loki.Runtime.Attributes
{
	[AttributeUsage(AttributeTargets.Field, AllowMultiple = false, Inherited = true)]
	public class OutputAttribute : Attribute
	{
		private readonly string m_Name;
		public string Name => m_Name;

		public OutputAttribute(string name)
		{
			m_Name = name;
		}
	}
}
