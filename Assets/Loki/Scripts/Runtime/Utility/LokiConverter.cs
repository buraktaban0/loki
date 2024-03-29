﻿using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Reflection;
using UnityEngine;

namespace Loki.Runtime.Utility
{
	public struct TypePair
	{
		public Type type1;
		public Type type2;

		public override bool Equals(object obj)
		{
			if (obj is TypePair other)
			{
				return other.type1 == type1 && other.type2 == type2;
			}

			return false;
		}

		public override int GetHashCode()
		{
			return type1.GetHashCode() ^ type2.GetHashCode();
		}
	}

	public abstract class LokiConverter
	{
		private static Dictionary<Type, HashSet<Type>> s_ImplicitNumericConversions =
			new Dictionary<Type, HashSet<Type>>
			{
				{
					typeof(sbyte),
					new HashSet<Type>
					{ typeof(short), typeof(int), typeof(long), typeof(float), typeof(double), typeof(decimal) }
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
					new HashSet<Type> { typeof(int), typeof(long), typeof(float), typeof(double), typeof(decimal) }
				},
				{
					typeof(ushort),
					new HashSet<Type>
					{
						typeof(int), typeof(uint), typeof(long), typeof(ulong), typeof(float), typeof(double),
						typeof(decimal)
					}
				},
				{ typeof(int), new HashSet<Type> { typeof(long), typeof(float), typeof(double), typeof(decimal) } },
				{
					typeof(uint),
					new HashSet<Type> { typeof(long), typeof(ulong), typeof(float), typeof(double), typeof(decimal) }
				},
				{ typeof(long), new HashSet<Type> { typeof(float), typeof(double), typeof(decimal) } },
				{
					typeof(char),
					new HashSet<Type>
					{
						typeof(ushort), typeof(int), typeof(uint), typeof(long), typeof(ulong), typeof(float),
						typeof(double), typeof(decimal)
					}
				},
				{ typeof(float), new HashSet<Type> { typeof(double) } },
				{ typeof(ulong), new HashSet<Type> { typeof(float), typeof(double), typeof(decimal) } },
			};

		private static Dictionary<TypePair, LokiConverter> s_CachedConverters =
			new Dictionary<TypePair, LokiConverter>();

		private static void SetConverterCache(Type fromType, Type toType, LokiConverter converter)
		{
			var pair = new TypePair { type1 = fromType, type2 = toType };
			s_CachedConverters.Add(pair, converter);
		}

		private static LokiConverter GetCachedConverter(Type fromType, Type toType)
		{
			var pair = new TypePair { type1 = fromType, type2 = toType };
			if (s_CachedConverters.TryGetValue(pair, out var converter))
			{
				return converter;
			}

			return null;
		}

		public static LokiConverter GetConverter(Type fromType, Type toType)
		{
			LokiConverter cachedConverter;
			if ((cachedConverter = GetCachedConverter(fromType, toType)) != null)
			{
				return cachedConverter;
			}

			if (fromType == toType || toType.IsAssignableFrom(fromType))
			{
				var converter = new NoOpConverter(fromType, toType);
				SetConverterCache(fromType, toType, converter);
				return converter;
			}

			if (!fromType.IsClass && !toType.IsClass &&
			    s_ImplicitNumericConversions.TryGetValue(fromType, out var set) &&
			    set.Contains(toType))
			{
				var converter = new NumericConverter(fromType, toType);
				SetConverterCache(fromType, toType, converter);
				return converter;
			}

			MethodInfo implicitCastMethod;
			if ((implicitCastMethod = GetImplicitCastMethod(definedOn: fromType, fromType, toType)) != null
			    || (implicitCastMethod = GetImplicitCastMethod(definedOn: toType, fromType, toType)) != null)
			{
				var converter = new ImplicitConverter(fromType, toType, implicitCastMethod);
				SetConverterCache(fromType, toType, converter);
				return converter;
			}

			TypeConverter tc;
			if ((tc = TypeDescriptor.GetConverter(fromType)).CanConvertTo(toType))
			{
				var converter = new TypeDescriptorConverter(fromType, toType, tc);
				SetConverterCache(fromType, toType, converter);
				return converter;
			}

			return null;
		}

		public static bool TryGetConverter(Type fromType, Type toType, out LokiConverter converter)
		{
			converter = GetConverter(fromType, toType);
			return converter != null;
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
			return implicitCastMethod.Invoke(null, new[] { obj });
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
