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
    }
}
