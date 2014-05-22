using System;
using System.Collections.Generic;
using System.ComponentModel.Composition;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using PA.Converters.Extensions;

namespace PA.Configuration
{
    public class ConfigurationItem : IConfigurationItem
    {
        public string Contract { get; private set; }

        protected IConfigurationSource source;

        public ConfigurationItem(string Contract, IConfigurationSource Source)
        {
            this.Contract = Contract;
            this.source = Source;
        }
    }

    public class ConfigurationItem<T> : ConfigurationItem, IConfigurationItem<T>
        where T: IConvertible
    {
        public T Value { get; set; }

        public ConfigurationItem(string contract, IConfigurationSource source)
            : base(contract, source)
        {
            this.Refresh();
        }

        public ConfigurationItem(string contract, IConfigurationSource source, T initialValue)
            : base(contract, source)
        {
            this.Value = initialValue;
        }

        public void Refresh()
        {
            this.Value = this.source.GetSetting(this.Contract).ParseTo<T,string>();
        }

        public void Save()
        {
            this.source.SetSetting(this.Contract, this.Value.ToString());
        }
    }
}
