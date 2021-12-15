using System;

namespace Loki.Runtime.Database
{
    public interface ITypeTargeting
    {
        public Type TargetedType { get; }
    }
}