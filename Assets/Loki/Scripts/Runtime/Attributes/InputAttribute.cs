using System;

namespace Loki.Runtime.Attributes
{
	[AttributeUsage(AttributeTargets.Field, AllowMultiple = false, Inherited = true)]
	public class InputAttribute : Attribute
	{
		private readonly string m_Name;
		public string Name => m_Name;

		public InputAttribute(string name)
		{
			m_Name = name;
		}
	}
}
