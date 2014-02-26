using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Runtime.Serialization;
using System.Reflection;
using System.Diagnostics;
using System.Runtime.Serialization.Formatters.Binary;
using System.Runtime.InteropServices;

namespace PA.Converters.Extensions
{
    public static class ObjectExtensions
    {
        public static T ParseTo<T, U>(this U value, [Optional] Type type)
        {
            Type t = type ?? typeof(T);

            if (!typeof(T).IsAssignableFrom(t))
            {
                throw new InvalidCastException("Cannot cast <" + type.FullName + "> to <T>");
            }

            if (t.IsEnum)
            {
                return (T) Enum.Parse(t, value.ToString());
            }
            else
            {
                T o = default(T);

                if (typeof(IConvertible).IsAssignableFrom(t))
                {
                    try
                    {
                        o = (T)Convert.ChangeType(value, t);
                    }
                    catch (Exception e)
                    {
                        Trace.TraceError(e.Message + "\n" + e.StackTrace);
                    }
                }

                if (o == null)
                {
                    ConstructorInfo ci = type.GetConstructor(new Type[] { typeof(U) });

                    if (ci is ConstructorInfo)
                    {
                        try
                        {
                            o = (T)ci.Invoke(new object[] { value });
                        }
                        catch (Exception e)
                        {
                            Trace.TraceError(e.Message + "\n" + e.StackTrace);
                        }
                    }
                }

                if (o == null)
                {
                    MethodInfo mi = t.GetMethod("Parse", new Type[] { typeof(string) });

                    if (mi is MemberInfo && mi.IsStatic)
                    {
                        o = (T)mi.Invoke(null, new object[] { value });
                    }
                }

                return o;
            }
        }

        public static IEnumerable<T> ParseTo<T, U>(this IEnumerable<U> value, [Optional] Type type)
        {
            foreach (U v in value)
            {
                yield return v.ParseTo<T, U>(type);
            }
        }




        [Obsolete]
        public static object ParseTo(this object value, Type type, bool ThrowError = false)
        {
            string str = value.ToString();
            Type[] targetTypes = new Type[] { value.GetType() };

            if (type.IsArray)
            {
                return str.Split(new char[] { str[0] }, StringSplitOptions.RemoveEmptyEntries).ParseTo(type, ThrowError);
            }
            else if (type.IsEnum)
            {
                return Enum.Parse(type, value.ToString());
            }
            else
            {
                object o = null;

                if (o == null && typeof(IConvertible).IsAssignableFrom(type))
                {
                    try
                    {
                        o = Convert.ChangeType(value, type);
                    }
                    catch (Exception e)
                    {
                        Trace.TraceError(e.Message + "\n" + e.StackTrace);

                        if (ThrowError)
                        {
                            throw new InvalidOperationException("Cannot parse/convert object " + value + " to <" + type.FullName + ">", e);
                        }
                    }
                }


                if (o == null)
                {
                    ConstructorInfo ci = type.GetConstructor(targetTypes);

                    if (ci is ConstructorInfo)
                    {
                        try
                        {
                            o = ci.Invoke(new object[] { value });
                        }
                        catch (Exception e)
                        {
                            Trace.TraceError(e.Message + "\n" + e.StackTrace);

                            if (ThrowError)
                            {
                                throw new InvalidOperationException("Cannot parse/convert object " + value + " to <" + type.FullName + ">", e);
                            }
                        }
                    }

                }

                if (o == null)
                {
                    MethodInfo mi = type.GetMethod("Parse", targetTypes);

                    if (mi is MemberInfo && mi.IsStatic)
                    {
                        o = mi.Invoke(null, new object[] { value });
                    }
                }

                return o;
            }
        }

        [Obsolete]
        internal static Array ParseTo(this object[] value, Type type, bool ThrowError = false)
        {
            if (!type.IsArray)
            {
                throw new ArgumentException("Type <" + type.FullName + "> is not an array");
            }

            Type eType = type.GetElementType();

            var obj = value
                .Select(s => s.ParseTo(eType, ThrowError))
                .Where(s => s != null)
                .ToArray();

            var arr = Array.CreateInstance(eType, obj.Length);
            Array.Copy(obj, arr, obj.Length);
            return arr;
        }
    }
}
