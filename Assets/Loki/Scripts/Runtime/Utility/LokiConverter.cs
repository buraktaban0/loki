using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Reflection;
using UnityEngine;

namespace Loki.Runtime.Utility
{
	public abstract class LokiConverter
	{
		private static Dictionary<Type, HashSet<Type>> implicitNumericConversions =
			new Dictionary<Type, HashSet<Type>>()
			{
				{
					typeof(sbyte),
					new HashSet<Type>
						{typeof(short), typeof(int), typeof(long), typeof(float), typeof(double), typeof(decimal)}
				},
				{
					typeof(byte),
					new HashSet<Type>
					{
						typeof(short), typeof(ushort), typeof(int), typeof(uint), typeof(long), typeof(ulong),
						typeof(float), typeof(double), typeof(decimal)
					}
				},
				{
					typeof(short),
					new HashSet<Type> {typeof(int), typeof(long), typeof(float), typeof(double), typeof(decimal)}
				},
				{
					typeof(ushort),
					new HashSet<Type>
					{
						typeof(int), typeof(uint), typeof(long), typeof(ulong), typeof(float), typeof(double),
						typeof(decimal)
					}
				},
				{typeof(int), new HashSet<Type> {typeof(long), typeof(float), typeof(double), typeof(decimal)}},
				{
					typeof(uint),
					new HashSet<Type> {typeof(long), typeof(ulong), typeof(float), typeof(double), typeof(decimal)}
				},
				{typeof(long), new HashSet<Type> {typeof(float), typeof(double), typeof(decimal)}},
				{
					typeof(char),
					new HashSet<Type>
					{
						typeof(ushort), typeof(int), typeof(uint), typeof(long), typeof(ulong), typeof(float),
						typeof(double), typeof(decimal)
					}
				},
				{typeof(float), new HashSet<Type> {typeof(double)}},
				{typeof(ulong), new HashSet<Type> {typeof(float), typeof(double), typeof(decimal)}},
			};

		public static LokiConverter GetConverter(Type fromType, Type toType)
		{
			if (fromType == toType)
				return new NoOpConverter(fromType, toType);

			if (toType.IsAssignableFrom(fromType))
				return new NoOpConverter(fromType, toType);


			if (!fromType.IsClass && !toType.IsClass && implicitNumericConversions.TryGetValue(fromType, out var set) &&
			    set.Contains(toType))
			{
				return new NumericConverter(fromType, toType);
			}

			MethodInfo implicitCastMethod;
			if ((implicitCastMethod = GetImplicitCastMethod(fromType, fromType, toType)) != null
			    || (implicitCastMethod = GetImplicitCastMethod(toType, fromType, toType)) != null)
			{
				return new ImplicitConverter(fromType, toType, implicitCastMethod);
			}

			TypeConverter tc;
			if ((tc = TypeDescriptor.GetConverter(fromType)).CanConvertTo(toType))
			{
				return new TypeDescriptorConverter(fromType, toType, tc);
			}

			return null;
		}


		private static MethodInfo GetImplicitCastMethod(Type definedOn, Type fromType, Type toType)
		{
			return definedOn.GetMethods(BindingFlags.Public | BindingFlags.Static).FirstOrDefault(m =>
			{
				if (m.Name != "op_Implicit" || !toType.IsAssignableFrom(m.ReturnType))
					return false;
				var parameters = m.GetParameters();
				return parameters.Length == 1 && parameters[0].ParameterType == fromType;
			});
		}


		public Type fromType;
		public Type toType;

		public abstract object Convert(object obj);
	}

	public class NoOpConverter : LokiConverter
	{
		public NoOpConverter(Type fromType, Type toType)
		{
			this.fromType = fromType;
			this.toType = toType;
		}

		public override object Convert(object obj)
		{
			return obj;
		}
	}

	public class NumericConverter : LokiConverter
	{
		public NumericConverter(Type fromType, Type toType)
		{
			this.fromType = fromType;
			this.toType = toType;
		}

		public override object Convert(object obj)
		{
			return System.Convert.ChangeType(obj, toType);
		}
	}

	public class ImplicitConverter : LokiConverter
	{
		public MethodInfo implicitCastMethod;

		public ImplicitConverter(Type fromType, Type toType, MethodInfo implicitCastMethod)
		{
			this.fromType = fromType;
			this.toType = toType;
			this.implicitCastMethod = implicitCastMethod;
		}

		public override object Convert(object obj)
		{
			return implicitCastMethod.Invoke(null, new[] {obj});
		}
	}

	public class TypeDescriptorConverter : LokiConverter
	{
		public TypeConverter tc;

		public TypeDescriptorConverter(Type fromType, Type toType, TypeConverter tc)
		{
			this.fromType = fromType;
			this.toType = toType;
			this.tc = tc;
		}

		public override object Convert(object obj)
		{
			return tc.ConvertTo(obj, toType);
		}
	}
}
