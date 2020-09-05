using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace Loki.Editor
{
	public class LokiFilter
	{
		public static readonly LokiFilter Class = new LokiFilter("class");
		public static readonly LokiFilter Method = new LokiFilter("method");
		public static readonly LokiFilter Variable = new LokiFilter("variable");

		public static LokiFilter FromType(Type type)
		{
			return new LokiFilter($"type{{{type.FullName}}}");
		}

		public static LokiFilter FromField(FieldInfo field)
		{
			return new LokiFilter($"field{{{field.FieldType.FullName}}}{{{field.Name}}}");
		}


		private readonly HashSet<string> set = new HashSet<string>();

		public LokiFilter(string filter)
		{
			set.Add(filter);
		}

		public LokiFilter(params string[] filters)
		{
			this.set.UnionWith(filters);
		}

		public LokiFilter(IEnumerable<string> filters)
		{
			this.set.UnionWith(set);
		}

		public LokiFilter(LokiFilter other)
		{
			this.set.UnionWith(other.set);
		}

		public void Add(string filter)
		{
			set.Add(filter);
		}

		public void Add(IEnumerable<string> filters) => set.UnionWith(filters);

		public bool IsSubsetOf(LokiFilter other)
		{
			return this.set.IsSubsetOf(other.set);
		}

		public bool Fits(string filter)
		{
			return this.set.Contains(filter);
		}

		public bool Fits(IEnumerable<string> filters)
		{
			return this.set.Intersect(filters).Any();
		}


		public override bool Equals(object obj)
		{
			if (obj is LokiFilter other)
				return this.IsSubsetOf(other) && other.IsSubsetOf(this);

			return false;
		}

		public override int GetHashCode()
		{
			return set.GetHashCode();
		}

		public static LokiFilter operator |(LokiFilter a, LokiFilter b)
		{
			return new LokiFilter(a.set.Union(b.set));
		}

		public static LokiFilter operator &(LokiFilter a, LokiFilter b)
		{
			return new LokiFilter(a.set.Intersect(b.set));
		}
	}
}
