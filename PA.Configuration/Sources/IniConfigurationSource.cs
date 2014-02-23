using System;
using System.ComponentModel;
using System.Configuration;
using System.Linq;
using System.Reflection;
using System.Diagnostics;
using System.Collections.Generic;

namespace PA.Configuration
{
    public partial class IniConfigurationSource : Component, IConfigurationSource
    {
        private Settings settings;

        public IniConfigurationSource()
        {
            this.settings = new Settings(Process.GetCurrentProcess().ProcessName + ".ini", Settings.Format.IniFormat);
            Trace.TraceInformation("Loading configuration from " + this.settings.FileName);
        }

        public bool ContainsSetting(string key)
        {
            lock (this.settings)
            {
                int k = key.IndexOf('/');

                if (k > 0)
                {
                    this.settings.BeginGroup(key.Substring(0, k));
                }

                bool s = this.settings.Contains(key.Substring(k + 1)) || this.settings.Contains(key.Substring(k + 1) + "/size");

                if (k > 0)
                {
                    this.settings.EndGroup();
                }

                return s;
            }
        }

        public bool ContainsSection(string name)
        {
            return false;
        }

        public string GetSetting(string key)
        {
            lock (this.settings)
            {
                int k = key.IndexOf('/');

                if (k > 0)
                {
                    this.settings.BeginGroup(key.Substring(0, k));
                }

                string s = this.settings.Value(key.Substring(k + 1));

                if (k > 0)
                {
                    this.settings.EndGroup();
                }

                return s;
            }
        }

        public string[] GetSettings(string section)
        {
            List<string> stg = new List<string>();

            lock (this.settings)
            {
                this.settings.BeginGroup(section);
                stg.AddRange(this.settings.ChildKeys());

                foreach (string group in this.settings.ChildGroups())
                {
                    if (this.settings.Contains(group + "/size"))
                    {
                        int size = this.settings.BeginReadArray(group);

                        for (int i = 0; i < size; i++)
                        {
                            this.settings.SetArrayIndex(i);

                            stg.AddRange(this.settings.ChildKeys().Select(k =>  group + "/" + i + (k.Length > 0 ? "/" + k : "")));

                            foreach (string subgroup in this.settings.ChildGroups())
                            {
                                stg.AddRange(this.GetSettings(subgroup + "/" + i).Select(k =>  group + "/" + i + (k.Length > 0 ? "/" + k : "")));
                            }
                        }

                        this.settings.EndArray();
                    }
                    else
                    {
                        stg.AddRange(this.GetSettings(group).Select(k =>  group + (k.Length > 0 ? "/" + k : "")));
                    }
                }

                this.settings.EndGroup();
            }

            return stg.Distinct().ToArray();
        }

        public void SetSetting(string key, string value)
        {
            lock (this.settings)
            {
                int k = key.IndexOf('/');

                if (k > 0)
                {
                    this.settings.BeginGroup(key.Substring(0, k));
                }

                this.settings.SetValue(key.Substring(k + 1), value);

                if (k > 0)
                {
                    this.settings.EndGroup();
                }

                this.settings.Sync();
            }
        }

        public object GetSection(string name)
        {
            lock (this.settings)
            {
                return null;
            }
        }

        public string[] GetSectionNames()
        {
            lock (this.settings)
            {
                return this.settings.ChildGroups();
            }
        }

    }
}