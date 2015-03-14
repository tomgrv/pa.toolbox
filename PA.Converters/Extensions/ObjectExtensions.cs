using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Runtime.Serialization;
using System.Reflection;
using System.Diagnostics;
using System.Runtime.InteropServices;

namespace PA.Converters.Extensions
{
    public static class ObjectExtensions
    {
        public static T ParseTo<T, U>(this U value, Type type = null)
        {
            Type t = type ?? typeof(T);

#if ! XAMARIN
             if (!typeof(T).IsAssignableFrom(t))     
#else
            if (!typeof(T).GetTypeInfo().IsAssignableFrom(t.GetTypeInfo()))
#endif
            {
                throw new InvalidCastException("Cannot cast <" + type.FullName + "> to <T>");
            }
#if ! XAMARIN
            if (t.IsEnum)
#else
            if (t.GetTypeInfo().IsEnum)
#endif
            {
                return (T)Enum.Parse(t, value.ToString(), true);
            }
            else
            {
                T o = default(T);
#if ! XAMARIN
                if (typeof(IConvertible).IsAssignableFrom(t))
                {
                    try
                    {
                        o = (T)Convert.ChangeType(value, t, CultureInfo.InvariantCulture);
                    }
                    catch (Exception e)
                    {
                        Debug.WriteLine(e.Message + "\n" + e.StackTrace);
                    }
                }
#endif

                if (o == null)
                {
#if ! XAMARIN
                    ConstructorInfo ci = t.GetConstructor(new Type[] { typeof(U) });
#else
                    ConstructorInfo ci = t.GetTypeInfo().DeclaredConstructors.Where(
                        c => c.GetParameters().Count() == 1 &&
                        c.GetParameters().First().ParameterType == typeof(U)
                    ).FirstOrDefault();

#endif
                    if (ci is ConstructorInfo)
                    {
                        try
                        {
                            o = (T)ci.Invoke(new object[] { value });
                        }
                        catch (Exception e)
                        {
                            Debug.WriteLine(e.Message + "\n" + e.StackTrace);
                        }
                    }
                }

                if (o == null)
                {
#if ! XAMARIN
                    MethodInfo mi = t.GetMethod("Parse", new Type[] { typeof(string) });
#else
                    MethodInfo mi = t.GetTypeInfo().GetDeclaredMethods("Parse").Where(
                        m => m.GetParameters().Count() == 1 &&
                        m.GetParameters().First().ParameterType == typeof(string)
                    ).FirstOrDefault();
#endif

                    if (mi is MethodInfo && mi.IsStatic)
                    {
                        o = (T)mi.Invoke(null, new object[] { value });
                    }
                }

                if (o == null)
                {

#if ! XAMARIN
                    MethodInfo mi = t.GetMethod("CreateFrom", new Type[] { typeof(string) });
#else
                    MethodInfo mi = t.GetTypeInfo().GetDeclaredMethods("CreateFrom").Where(
                        m => m.GetParameters().Count() == 1 &&
                        m.GetParameters().First().ParameterType == typeof(string)
                    ).FirstOrDefault();
#endif
                    if (mi is MemberInfo && mi.IsStatic)
                    {
                        o = (T)mi.Invoke(null, new object[] { value });
                    }
                }

                return o;
            }
        }

        public static IEnumerable<T> ParseTo<T, U>(this IEnumerable<U> value, Type type = null)
        {
            foreach (U v in value)
            {
                yield return v.ParseTo<T, U>(type);
            }
        }

        public static Array ToArray<T>(this IEnumerable<T> value, Type type)
        {

            Array source = value.Where(
                s => s != null &&
#if ! XAMARIN
                type.IsAssignableFrom(s.GetType())
#else
                type.GetTypeInfo().IsAssignableFrom(s.GetType().GetTypeInfo())
#endif
            ).ToArray();

            Array destination = Array.CreateInstance(type, source.Length);
            Array.Copy(source, destination, source.Length);
            return destination;
        }
    }
}
