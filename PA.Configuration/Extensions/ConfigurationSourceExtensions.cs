using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.ComponentModel.Composition.Primitives;
using System.ComponentModel.Composition.ReflectionModel;
using System.Reflection;
using PA.Converters.Extensions;
using System.Diagnostics;
using System.ComponentModel.Composition.Hosting;

namespace PA.Configuration
{
    public static class ConfigurationSourceExtensions
    {

        public static IEnumerable<string> GetArrays<T>(this T source, string section)
             where T : IConfigurationSource
        {
            return source.GetSettings(section).Where(s => s.EndsWith("/size")).Select(s => s.Substring(0,s.LastIndexOf('/'))).ToArray();
        }

        public static int GetArraySize<T>(this T source, string section, string name)
           where T : IConfigurationSource
        {
            return int.Parse(source.GetSetting(section + "/" + name + "/size"));
        }

        public static IEnumerable<string> GetArraySettings<T>(this T source, string section, string name)
            where T : IConfigurationSource
        {
            foreach (string array in source.GetArrays(section).ToArray())
            {
                int size =  source.GetArraySize(section,array);

                for (int i = 0; i <size; i++)
                {
                    foreach (string setting in source.GetSettings(section + "/" + name + "/" + i))
                    {
                        if (setting.Length > 0)
                        {
                            yield return i + "/" + setting;
                        }
                        else
                        {
                            yield return i.ToString();
                        }
                    }
                }
            }
        }

    }
}
