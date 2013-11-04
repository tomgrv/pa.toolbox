using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Runtime.Serialization;
using System.Reflection;
using System.Diagnostics;

namespace PA.Converters.Extensions
{
    public static class ObjectExtensions
    {
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
