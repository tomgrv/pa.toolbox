using System;
namespace PA.Configuration
{
    public interface IConfigurationSource
    {
        bool ContainsSetting(string key);
        string GetSetting(string key);
        string[] GetSettings(string section);
        void SetSetting(string key, string value);
        bool ContainsSection(string name);
        object GetSection(string name);
        string[] GetSectionNames();
    }
}