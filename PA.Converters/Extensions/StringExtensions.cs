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
    public static class StringExtensions
    {
        public static string[] AsArray(this string value)
        {
            return value.Split(new char[] { value[0] }, StringSplitOptions.RemoveEmptyEntries);
        }

        public static Array AsArray<T>(this string value, Type TargetType, Func<Type, string, T> getInstance)
        {
            if (TargetType.HasElementType)
            {
                Type type = TargetType.GetElementType();

                if (type.HasElementType)
                {
                    return value.AsArray().Cast<string>().Select(a => a.AsArray(type, (t, s) => getInstance(t, s))).ToArray(type);
                }
                else
                {
                    return value.AsArray().Cast<string>().Select(s => getInstance(type, s)).ToArray(type);
                }
            }

            throw new InvalidCastException("Not an array");
        }
    }
}
