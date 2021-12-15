using System;
using Loki.Runtime.Database;

namespace Loki.Runtime.Attributes
{
    [AttributeUsage(AttributeTargets.Class)]
    public class GraphRendererAttribute : Attribute, ITypeTargeting
    {
        public Type TargetedType => m_Type;

        private Type m_Type;

        public GraphRendererAttribute(Type type)
        {
            m_Type = type;
        }
    }
}