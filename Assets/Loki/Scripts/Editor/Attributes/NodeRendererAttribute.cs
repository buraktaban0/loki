using System;
using Loki.Runtime.Database;

namespace Loki.Runtime.Attributes
{
    [AttributeUsage(AttributeTargets.Class)]
    public class NodeRendererAttribute : Attribute, ITypeTargeting
    {
        public Type TargetedType => m_Type;

        private Type m_Type;

        public NodeRendererAttribute(Type type)
        {
            m_Type = type;
        }
    }
}