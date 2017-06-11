#region License

/* Copyright © 2017, Dahomey Technologies and Contributors
 * For conditions of distribution and use, see copyright notice in license.txt file
 */

#endregion

using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace Dahomey.ExpressionEvaluator
{
    public static class ReflectionHelper
    {
        public static Func<T, TP> CreateDelegate<T, TP>(PropertyInfo propertyInfo)
        {
            return (Func<T, TP>)Delegate.CreateDelegate(typeof(Func<T, TP>), propertyInfo.GetGetMethod());
        }

        public static Func<TR> CreateDelegate<TR>(MethodInfo methodInfo, object target = null)
        {
            return (Func<TR>)Delegate.CreateDelegate(typeof(Func<TR>), target, methodInfo);
        }

        public static Func<T1, TR> CreateDelegate<T1, TR>(MethodInfo methodInfo, object target = null)
        {
            return (Func<T1, TR>)Delegate.CreateDelegate(typeof(Func<T1, TR>), target, methodInfo);
        }

        public static Func<T1, T2, TR> CreateDelegate<T1, T2, TR>(MethodInfo methodInfo, object target = null)
        {
            return (Func<T1, T2, TR>)Delegate.CreateDelegate(typeof(Func<T1, T2, TR>), target, methodInfo);
        }

        public static Func<T1, T2, T3, TR> CreateDelegate<T1, T2, T3, TR>(MethodInfo methodInfo, object target = null)
        {
            return (Func<T1, T2, T3, TR>)Delegate.CreateDelegate(typeof(Func<T1, T2, T3, TR>), target, methodInfo);
        }

        public static Func<T1, T2, T3, T4, TR> CreateDelegate<T1, T2, T3, T4, TR>(MethodInfo methodInfo, object target = null)
        {
            return (Func<T1, T2, T3, T4, TR>)Delegate.CreateDelegate(typeof(Func<T1, T2, T3, T4, TR>), target, methodInfo);
        }

        public static bool IsList(Type type)
        {
            return type.IsGenericType && type.GetGenericTypeDefinition() == typeof(List<>);
        }

        public static bool IsNumberList(Type type)
        {
            Type itemType;

            if (type.IsArray)
            {
                itemType = type.GetElementType();
            }
            else if (IsList(type))
            {
                itemType = type.GetGenericArguments()[0];
            }
            else
            {
                return false;
            }

            return IsNumber(itemType);
        }

        public static bool IsNumber(Type type)
        {
            return type == typeof(sbyte)
                || type == typeof(byte)
                || type == typeof(short)
                || type == typeof(ushort)
                || type == typeof(int)
                || type == typeof(uint)
                || type == typeof(long)
                || type == typeof(ulong)
                || type == typeof(float)
                || type == typeof(double)
                || type.IsEnum;
        }

        public static Type GetType(IEnumerable<Assembly> assemblies, string name)
        {
            return assemblies
                .SelectMany(a => a.GetTypes())
                .FirstOrDefault(t => t.Name == name);
        }

        public static Func<T, double> GenerateConverter<T>()
        {
            string methodName;

            if (typeof(T).IsEnum)
            {
                // Hack: we force cast Func<int, double> to Func<TEnum, double>
                methodName = "Int32ToDouble";
            }
            else if (typeof(T) == typeof(sbyte))
            {
                methodName = "SByteToDouble";
            }
            else if (typeof(T) == typeof(byte))
            {
                methodName = "ByteToDouble";
            }
            else if (typeof(T) == typeof(short))
            {
                methodName = "Int16ToDouble";
            }
            else if (typeof(T) == typeof(ushort))
            {
                methodName = "UInt16ToDouble";
            }
            else if (typeof(T) == typeof(int))
            {
                methodName = "Int32ToDouble";
            }
            else if (typeof(T) == typeof(uint))
            {
                methodName = "UInt32ToDouble";
            }
            else if (typeof(T) == typeof(long))
            {
                methodName = "Int64ToDouble";
            }
            else if (typeof(T) == typeof(ulong))
            {
                methodName = "UInt64ToDouble";
            }
            else if (typeof(T) == typeof(float))
            {
                methodName = "SingleToDouble";
            }
            else if (typeof(T) == typeof(double))
            {
                methodName = "DoubleToDouble";
            }
            else
            {
                throw new NotSupportedException(string.Format("Cannot convert type {0} to double", typeof(T).Name));
            }

            MethodInfo methodInfo = typeof(ReflectionHelper).GetMethod(methodName, BindingFlags.Static | BindingFlags.NonPublic);
            return CreateDelegate<T, double>(methodInfo);
        }

        public static Func<double, T> GenerateFromDoubleConverter<T>()
        {
            string methodName;

            if (typeof(T).IsEnum)
            {
                // Hack: we force cast Func<double, int> to Func<double, TEnum>
                methodName = "DoubleToInt32";
            }
            else if (typeof(T) == typeof(sbyte))
            {
                methodName = "DoubleToSByte";
            }
            else if (typeof(T) == typeof(byte))
            {
                methodName = "DoubleToByte";
            }
            else if (typeof(T) == typeof(short))
            {
                methodName = "DoubleToInt16";
            }
            else if (typeof(T) == typeof(ushort))
            {
                methodName = "DoubleToUInt16";
            }
            else if (typeof(T) == typeof(int))
            {
                methodName = "DoubleToInt32";
            }
            else if (typeof(T) == typeof(uint))
            {
                methodName = "DoubleToUInt32";
            }
            else if (typeof(T) == typeof(long))
            {
                methodName = "DoubleToInt64";
            }
            else if (typeof(T) == typeof(ulong))
            {
                methodName = "DoubleToUInt64";
            }
            else if (typeof(T) == typeof(float))
            {
                methodName = "DoubleToSingle";
            }
            else if (typeof(T) == typeof(double))
            {
                methodName = "DoubleToDouble";
            }
            else
            {
                throw new NotSupportedException();
            }

            MethodInfo methodInfo = typeof(ReflectionHelper).GetMethod(methodName, BindingFlags.Static | BindingFlags.NonPublic);
            return CreateDelegate<double, T>(methodInfo);
        }

        private static double SByteToDouble(sbyte value)
        {
            return value;
        }

        private static double ByteToDouble(byte value)
        {
            return value;
        }

        private static double Int16ToDouble(short value)
        {
            return value;
        }

        private static double UInt16ToDouble(ushort value)
        {
            return value;
        }

        private static double Int32ToDouble(int value)
        {
            return value;
        }

        private static double UInt32ToDouble(uint value)
        {
            return value;
        }

        private static double Int64ToDouble(long value)
        {
            return value;
        }

        private static double UInt64ToDouble(ulong value)
        {
            return value;
        }

        private static double SingleToDouble(float value)
        {
            return value;
        }

        private static double DoubleToDouble(double value)
        {
            return value;
        }

        private static sbyte DoubleToSByte(double value)
        {
            return (sbyte)value;
        }

        private static byte DoubleToByte(double value)
        {
            return (byte)value;
        }

        private static short DoubleToInt16(double value)
        {
            return (short)value;
        }

        private static ushort DoubleToUInt16(double value)
        {
            return (ushort)value;
        }

        private static int DoubleToInt32(double value)
        {
            return (int)value;
        }

        private static uint DoubleToUInt32(double value)
        {
            return (uint)value;
        }

        private static long DoubleToInt64(double value)
        {
            return (long)value;
        }

        private static ulong DoubleToUInt64(double value)
        {
            return (ulong)value;
        }

        private static float DoubleToSingle(double value)
        {
            return (float)value;
        }
    }
}
