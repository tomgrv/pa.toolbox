using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.Composition.Hosting;
using System.ComponentModel.Composition.Primitives;
using System.ComponentModel.Composition.ReflectionModel;
using System.Configuration;
using System.Linq;
using System.Reflection;
using System.Text;
using PA.Converters.Extensions;
using System.Diagnostics;
using PA.Configuration;

namespace PA.Configuration
{
    public class ConfigurationItemExportProvider : ExportProvider, IConfigurationProvider
    {
        public IConfigurationSource Source { get; private set; }

        public ConfigurationItemExportProvider(IConfigurationSource configurationSource)
        {
            // the configuration source determines where configuration values come from (eg. App.Config file)
            this.Source = configurationSource;
        }

        protected override IEnumerable<Export> GetExportsCore(ImportDefinition definition, AtomicComposition atomicComposition)
        {
            lock (this.Source)
            {
                foreach (Export e in this.GetConfigurationItems(definition)
                    .GetExports<object>(definition, (t,s) => this.GetInstance(definition, t, s)))
                {
                    yield return e;
                }
            }
        }

        private object GetInstance(ImportDefinition definition, Type type, string configvalue)
        {
            if (typeof(ConfigurationItem).IsAssignableFrom(type) && type.IsGenericType)
            {
                return Activator.CreateInstance(type, definition.ContractName, this.Source);
            }

            object value = configvalue.ParseTo<object, string>(type);

            if (value is object)
            {
                return value;
            }

            return null;
        }
    }
}
