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

        public static string ToPascalCase(this string str)
        {
            // If there are 0 or 1 characters, just return the string.
            if (str == null)
                return str;
            if (str.Length < 2)
                return str.ToUpper();

            // Split the string into words.
            string[] words = str.Split(new char[] { }, StringSplitOptions.RemoveEmptyEntries);

            // Combine the words.
            string result = "";
            foreach (string word in words)
            {
                result += word.Substring(0, 1).ToUpper() + word.Substring(1);
            }
            return result;
        }

        public static string ToCamelCase(this string str)
        {
            // If there are 0 or 1 characters, just return the string.
            if (str == null || str.Length < 2)
                return str;

            // Split the string into words.
            string[] words = str.Split(new char[] { }, StringSplitOptions.RemoveEmptyEntries);

            // Combine the words.
            string result = words[0].ToLower();
            for (int i = 1; i < words.Length; i++)
            {
                result += words[i].Substring(0, 1).ToUpper() + words[i].Substring(1);
            }

            return result;
        }
    }
}
