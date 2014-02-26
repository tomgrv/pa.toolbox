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
                return (T)Enum.Parse(t, value.ToString());
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

                if (o == null)
                {
                    MethodInfo mi = t.GetMethod("CreateFrom", new Type[] { typeof(string) });

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

        public static Array ToArray<T>(this IEnumerable<T> value, Type type)
        {
            Array source = value.Where(s => s != null && type.IsAssignableFrom(s.GetType())).ToArray();
            Array destination = Array.CreateInstance(type, source.Length);
            Array.Copy(source, destination, source.Length);
            return destination;
        }
    }
}
