using System;
using System.ComponentModel;
using System.Configuration;
using System.Linq;

namespace PA.Configuration
{
    public partial class FileConfigurationSource : Component, IConfigurationSource
    {
        public void BeginInit()
        {
            
        }

        public void EndInit()
        {
           
        }

        public bool ContainsSetting( string key)
        {
            return ConfigurationManager.AppSettings.AllKeys.Contains(key);
        }

        public bool ContainsSection(string name)
        {
            return ConfigurationManager.GetSection(name) != null;
        }

        public string GetSetting( string key)
        {
            return ConfigurationManager.AppSettings[key];
        }

        public string[] GetSettings(string section)
        {
            return ConfigurationManager.AppSettings.Keys.Cast<string>().ToArray();
        }

        public void SetSetting(string key, string value)
        {
            throw new NotImplementedException();
        }

        public object GetSection( string name)
        {
            return ConfigurationManager.GetSection(name);
        }

        public string[] GetSectionNames()
        {
            throw new NotImplementedException();
        }
    }
}